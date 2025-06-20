using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.EntityFrameworkCore;
using UsersService.Src.Application.Commands.Concretes;
using UsersService.Src.Application.Commands.Concretes.BankPayment;
using UsersService.Src.Application.Commands.Data;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.DTOs.Update;
using UsersService.Src.Application.Interfaces;
using UsersService.Src.Application.Mapping;
using UsersService.Src.Application.Options;
using UsersService.Src.Application.Services;
using UsersService.Src.Domain.Interfaces;
using UsersService.Src.Infraestructure.Data;
using UsersService.Src.Infraestructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBankPaymentDataRepository, BankPaymentDataRepository>();
builder.Services.AddScoped<IBankPaymentDataService, BankPaymentDataService>();

builder.Services.AddScoped<ICommand<(string, string), LoggedUserDTO?>, LoginUserCommand>();
builder.Services.AddScoped<ICommand<(Guid PublicId, UpdateUserRequestDTO Request), bool>, UpdateUserCommand>();
builder.Services.AddScoped<ICommand<string, LoggedUserDTO?>, GetLoggedUserCommand>();
builder.Services.AddScoped<ICommand<Guid, UserDTO?>, GetUserByPublicIdCommand>();
builder.Services.AddScoped<ICommand<string, string?>, RefreshAccessTokenCommand>();
builder.Services.AddScoped<ICommand<string, bool>, ValidateAccessTokenCommand>();
builder.Services.AddScoped<ICommand<string?, bool>, LogoutUserCommand>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var cognitoClient = provider.GetRequiredService<AmazonCognitoIdentityProviderClient>();
    var clientId = config["AWS:Cognito:ClientId"];
#pragma warning disable CS8604 // Possible null reference argument.
    return new LogoutUserCommand(cognitoClient, clientId);
#pragma warning restore CS8604 // Possible null reference argument.
});
builder.Services.AddScoped<ICommand<BankPaymentInput, bool>, CreateBankPaymentDataCommand>();
builder.Services.AddScoped<ICommand<BankPaymentInput, bool>, UpdateBankPaymentDataCommand>();

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var region = Amazon.RegionEndpoint.GetBySystemName(config["AWS:Cognito:Region"]);
    return new AmazonCognitoIdentityProviderClient(region);
});

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var client = provider.GetRequiredService<AmazonCognitoIdentityProviderClient>();
    return new CognitoUserPool(
        config["AWS:Cognito:UserPoolId"],
        config["AWS:Cognito:ClientId"],
        client);
});
builder.Services.Configure<CognitoSettings>(
    builder.Configuration.GetSection("AWS:Cognito"));

builder.Services.AddAutoMapper(typeof(UserProfile));

var app = builder.Build();
app.MapControllers();
app.UseCors("AllowFrontEnd");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
