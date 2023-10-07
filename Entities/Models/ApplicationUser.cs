using Microsoft.AspNetCore.Identity;
using Shared.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime Birthday { get; set; }
    public string? Gender { get; set; }
    public bool? IsEmployed { get; set; }
    public MartialStatus? MartialStatus { get; set; }
    public string? Birthplace { get; set; }
    public string? ThePassword { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; } 
    public string? TokenHash { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateModified { get; set; }
}