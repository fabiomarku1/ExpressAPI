using Repository.Contracts;

namespace Contracts;

public interface IRepositoryManager
{
    IUserRepository UserRepository { get; }
    Task SaveAsync();

}