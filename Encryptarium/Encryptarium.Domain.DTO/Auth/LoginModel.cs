using System.ComponentModel.DataAnnotations;

namespace Encryptarium.Domain.DTO.Auth;

public class LoginModel
{
    [Required(ErrorMessage = "Поле UserName является обязательным")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Поле Password является обязательным")]
    public string Password { get; set; }
}
