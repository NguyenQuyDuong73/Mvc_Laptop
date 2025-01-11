using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models
{
    public class Permission
    {
        public Permission(string functionId, string roleId, string commandId)
        {
            FunctionId = functionId;
            RoleId = roleId;
            CommandId = commandId;
        }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string FunctionId { get; set; }
        public Function? Function { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string RoleId { get; set; }
        public Role? Role { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string CommandId { get; set; }
        public Command? Command { get; set; }
    }
}