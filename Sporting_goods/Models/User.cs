using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sporting_goods.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string? UserSurname { get; set; }
        public string? UserName { get; set; }
        public string? UserPatronymic { get; set; }
        public string? UserLogin { get; set; }
        public string? UserPassword { get; set; }
        public int? UserRole { get; set; }
        public Role? Role { get; set; }
    }
}