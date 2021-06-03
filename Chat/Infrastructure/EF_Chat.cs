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
    public class EF_Chat : IChat
    {
        private readonly ChatContext db;
        public EF_Chat(ChatContext chatContext) => db = chatContext;

        public async Task<ChatHub> AddChatAsync(ChatHub chatHub)
        {
            db.Chats.Add(chatHub);
            await db.SaveChangesAsync();
            return await db.Chats.Where(x => x.FirstUser == chatHub.FirstUser && x.SecondUser == chatHub.SecondUser).FirstOrDefaultAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            //ChatHub chat = await db.Chats.Include(x => x.Messages).Where(x => x.ID == ChatID).FirstOrDefaultAsync();
            //chat.Messages.Add(message);
            db.Messages.Add(message);
            await db.SaveChangesAsync();
        }

        public async Task<ChatHub> GetChatAsync(int id) => await db.Chats.Where(x => x.ID == id).FirstOrDefaultAsync();
        public async Task<ChatHub> GetChatAsync(string FirstUser, string SecondUser) =>
            await db.Chats.Where(x => x.FirstUser == FirstUser && x.SecondUser == SecondUser).FirstOrDefaultAsync();

        public async Task<List<Message>> GetMessagesAsync(int ChatID)
        {
            //ChatHub chat = await db.Chats.Include(x => x.Messages).Where(x => x.ID == ChatID).FirstOrDefaultAsync();
            //if(chat != null)
            //    return chat.Messages
            //return null;
            return await db.Messages.Include(x => x.Sender).Where(x => x.ChatHubID == ChatID).OrderByDescending(x => x.DateTime).ToListAsync();
        }

        public Dictionary<string, int> GetNewMessagesCount(string EMail)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach(var u in db.Chats.Include(x => x.Messages))
            {
                string tempMail;
                if (u.FirstUser == EMail)
                    tempMail = u.SecondUser;
                else if (u.SecondUser == EMail)
                    tempMail = u.FirstUser;
                else
                    continue;

                dictionary.Add(tempMail, u.Messages.Where(x => x.Sender.EMail != EMail && !x.Checked).Count());
            }

            return dictionary;
        }

        public async Task SetMessageCheckedAsync(int id)
        {
            Message message = await db.Messages.Where(x => x.ID == id).FirstOrDefaultAsync();
            if(message != null)
                message.Checked = true;
            await db.SaveChangesAsync();
        }
    }
}
