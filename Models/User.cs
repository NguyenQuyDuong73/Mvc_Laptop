using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MvcLaptop.Models;

public class User : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }
    [PersonalData]
    public DateTime DOB { get; set; }
    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    public User()
    { }
    public User(string userName, string password, string email)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }
}