using Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Interfaces
{
    public interface IUser
    {
        Task<List<User>> GetUsersAsync();
        List<User> GetUsers();
        Task<User> GetUserAsync(LoginViewModel lv);
        Task<User> GetUserAsync(int id);
        Task AddUserAsync(User user);
    }
}
