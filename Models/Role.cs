using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MvcLaptop.Models;

public class Role : IdentityRole
{
    public ICollection<Permission>? Permissions { get; set; }
}