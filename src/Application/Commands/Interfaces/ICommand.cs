namespace UsersService.Src.Application.Commands.Interfaces;

public interface ICommand<TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input);
}
