using Shared.Types;

namespace Shared.DTO;

public class UpdateUserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string Gender { get; set; }
    public bool? IsEmployed { get; set; }
    public MartialStatus? MartialStatus { get; set; }
    public string Birthplace { get; set; }
}