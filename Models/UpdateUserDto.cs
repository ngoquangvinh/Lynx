using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynxUI_Main.Models
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }  // ⬅️ BẮT BUỘC phải có!
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string AvatarUrl { get; set; } = "/Assets/avatar_default.png";

    }
}
