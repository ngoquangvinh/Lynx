using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynxUI_Main.Models
{
    public class RegisterModel
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? Birthday { get; set; }
    }

    public class LoginModel
    {
        public required string UsernameOrEmail { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordModel
    {
        public required string Email { get; set; }
    }
}
