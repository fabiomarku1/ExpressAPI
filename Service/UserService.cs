using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;
using Shared.DTO;


namespace Service;
public class UserService : IUserService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryManager _repositoryManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ILoggerManager logger, IMapper mapper, IRepositoryManager repositoryManager, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _mapper = mapper;
        _repositoryManager = repositoryManager;
        _userManager = userManager;
    }


    public async Task<bool> CreateUser(CreateUserDTO request)
    {

        var existingUser = await _repositoryManager.UserRepository.GetRecordByUsername(request.Username);
        if (existingUser is not null) throw new BadRequestException($"Username {request.Username} already exisits");


         existingUser = _mapper.Map<ApplicationUser>(request);

        existingUser.DateCreated = DateTime.Now;
        existingUser.ThePassword = request.Password;

        IdentityResult result = null;

        existingUser.ThePassword = request.Password;
        result = await _userManager.CreateAsync(existingUser, existingUser.ThePassword);

        if (!result.Succeeded)
        {
            var errorDetailsStr = string.Join("|", result.Errors.Select(x => x.Description));
            throw new BadRequestException(errorDetailsStr);
        }
        await _userManager.AddToRoleAsync(existingUser, "User");
        await _repositoryManager.SaveAsync();


        return true;
    }

    public async Task<IEnumerable<GetUsersListDTO>> GetUsers()
    {
        var list = await _repositoryManager.UserRepository.GetAllUsers();
        if (list is null) throw new BadRequestException("No list of users was found!");

        var mapped = _mapper.Map<IEnumerable<GetUsersListDTO>>(list);
        return mapped;
    }

    public async Task<IEnumerable<GetUsersListDTO>> GetUsersByPattern(string searchPattern)
    {
        var list = await _repositoryManager.UserRepository.GetUsersByPattern(searchPattern);
        if (list is null) throw new BadRequestException("No list of users was found!");

        var mapped = _mapper.Map<IEnumerable<GetUsersListDTO>>(list);
        return mapped;
    }

    public async Task<GetUserDetailsDTO> GetUserDetails(int userId)
    {
        var user = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (user is null) throw new NotFoundException("No user was found !");

        return _mapper.Map<GetUserDetailsDTO>(user);
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var user = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (user is null) throw new BadRequestException("no user was found!");

        _repositoryManager.UserRepository.DeleteRecord(user);
        await _repositoryManager.SaveAsync();
        return true;
    }

    public async Task<bool> UpdateUser(int userId, UpdateUserDTO request)
    {
        var user = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (user is null) throw new BadRequestException("no user was found!");

        _mapper.Map(request,user );

        user.DateModified = DateTime.Now;

        _repositoryManager.UserRepository.UpdateRecord(user);
        await _repositoryManager.SaveAsync();
        return true;

    }
}

