using LynxUI_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynxUI_Main.Helpers
{
    public static class UserItemMapper
    {
        public static UserItem ToUserItem(UserItemDto dto)
        {
            return new UserItem
            {
                Id = dto.UserId,
                DisplayName = dto.UserName,
                FullName = dto.FullName,
                AvatarUrl = dto.AvatarUrl,
                IsOnline = dto.IsOnline,
                 // bổ sung:
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Birthday = dto.Birthday
            };
        }
    }

}
