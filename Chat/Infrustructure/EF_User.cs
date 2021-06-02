using Chat.Interfaces;
using Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrustructure
{
    public class EF_User : IUser
    {
        private readonly ChatContext db;
        public EF_User(ChatContext chatContext) => db = chatContext;

        public async Task AddUserAsync(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(LoginViewModel lv) => await db.Users.Where(x => x.EMail == lv.Email).FirstOrDefaultAsync();
        public async Task<User> GetUserAsync(int id) => await db.Users.Where(x => x.ID == id).FirstOrDefaultAsync();

        public List<User> GetUsers() => db.Users.ToList();

        public async Task<List<User>> GetUsersAsync() => await db.Users.ToListAsync();
    }
}
