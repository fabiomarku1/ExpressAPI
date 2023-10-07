using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Repository.Contracts;

namespace Repository;
public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;

    private readonly Lazy<IUserRepository> _userRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
    }

    public IUserRepository UserRepository => _userRepository.Value;
   
    public async Task SaveAsync()
    {
        _repositoryContext.ChangeTracker.AutoDetectChangesEnabled = false;
        await _repositoryContext.SaveChangesAsync();
    }
}

