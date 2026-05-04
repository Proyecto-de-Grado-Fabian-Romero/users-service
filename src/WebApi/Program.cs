using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.EntityFrameworkCore;
using UsersService.src.Application.Commands.Concretes;
using UsersService.Src.Application.Commands.Concretes;
using UsersService.Src.Application.Commands.Concretes.BankPayment;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.DTOs.Update;
using UsersService.Src.Application.Interfaces;
using UsersService.Src.Application.Mapping;
using UsersService.Src.Application.Options;
using UsersService.Src.Application.Services;
using UsersService.Src.Domain.Interfaces;
using UsersService.Src.Infraestructure.Data;
using UsersService.Src.Infraestructure.Messaging;
using UsersService.Src.Infraestructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            if (allowedOrigins != null && allowedOrigins.Length > 0)
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
            else
            {
                policy
                    .WithOrigins("http://localhost:3000", "https://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        }
    );
});

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<UsersService.Src.Infraestructure.Messaging.RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq")
);

builder.Services.AddSingleton<
    UsersService.Src.Infraestructure.Messaging.INotificationsPublisher,
    UsersService.Src.Infraestructure.Messaging.RabbitMqNotificationsPublisher
>();
builder.Services.AddSingleton<IRabbitMqChannelAccessor, RabbitMqHostedConnection>();
builder.Services.AddHostedService(sp =>
    (RabbitMqHostedConnection)sp.GetRequiredService<IRabbitMqChannelAccessor>()
);
builder.Services.AddSingleton<IFcmTokenPublisher, RabbitMqFcmTokenPublisher>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBankPaymentDataRepository, BankPaymentDataRepository>();
builder.Services.AddScoped<IBankPaymentDataService, BankPaymentDataService>();

builder.Services.AddScoped<ICommand<LoginRequest, LoggedUserDTO?>, LoginUserCommand>();
builder.Services.AddScoped<
    ICommand<(Guid PublicId, UpdateUserRequestDTO Request), bool>,
    UpdateUserCommand
>();
builder.Services.AddScoped<ICommand<string, LoggedUserDTO?>, GetLoggedUserCommand>();
builder.Services.AddScoped<ICommand<Guid, UserDTO?>, GetUserByPublicIdCommand>();
builder.Services.AddScoped<ICommand<string, string?>, RefreshAccessTokenCommand>();
builder.Services.AddScoped<ICommand<string, bool>, ValidateAccessTokenCommand>();
builder.Services.AddScoped<ICommand<SignUpRequest, bool>, SignUpUserCommand>();
builder.Services.AddScoped<ICommand<ConfirmSignUpRequest, bool>, ConfirmSignUpCommand>();
builder.Services.AddScoped<ICommand<ResendCodeRequest, bool>, ResendConfirmationCodeCommand>();
builder.Services.AddScoped<
    ICommand<(string, ChangePasswordRequest), bool>,
    ChangePasswordCommand
>();
builder.Services.AddScoped<ICommand<ForgotPasswordRequest, bool>, ForgotPasswordCommand>();
builder.Services.AddScoped<
    ICommand<ConfirmForgotPasswordRequest, bool>,
    ConfirmForgotPasswordCommand
>();
builder.Services.AddScoped<ICommand<string?, bool>, LogoutUserCommand>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var cognitoClient = provider.GetRequiredService<AmazonCognitoIdentityProviderClient>();
    var clientId = config["AWS:Cognito:ClientId"];
#pragma warning disable CS8604
    return new LogoutUserCommand(cognitoClient, clientId);
#pragma warning restore CS8604
});
builder.Services.AddScoped<CreateBankPaymentDataCommand>();
builder.Services.AddScoped<UpdateBankPaymentDataCommand>();

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
        client
    );
});
builder.Services.Configure<CognitoSettings>(builder.Configuration.GetSection("AWS:Cognito"));

builder.Services.AddAutoMapper(typeof(UserProfile));

var app = builder.Build();

// ✅ Orden correcto del pipeline
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
