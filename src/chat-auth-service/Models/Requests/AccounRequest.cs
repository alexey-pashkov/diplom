using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Requsets;

public class AccountRequest
{
    [Required]
    public string Login {get; set;} = null!;
    [Required]
    public string Password {get; set;} = null!;
}  