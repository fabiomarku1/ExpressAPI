using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Repository.Contracts;
public interface IUserRepository
{
    void CreateRecord(ApplicationUser user);
    void UpdateRecord(ApplicationUser user);
    void DeleteRecord(ApplicationUser user);
    Task<IEnumerable<ApplicationUser>> GetAllUsers();
    Task<IEnumerable<ApplicationUser>> GetUsersByPattern(string searchPattern);
    Task<ApplicationUser> GetRecordByUsername(string username);
    Task<ApplicationUser> GetRecordById(int userId);
    Task<ApplicationUser> GetRecordByEmail(string email);

}

