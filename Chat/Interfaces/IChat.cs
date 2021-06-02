using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Models;

namespace Chat.Interfaces
{
    public interface IChat
    {
        Task<ChatHub> GetChatAsync(int id);
        Task<ChatHub> GetChatAsync(string FirstUser, string SecondUser);
        Task<ChatHub> AddChatAsync(ChatHub chatHub);

        Task<List<Message>> GetMessagesAsync(int ChatID);
        Task AddMessageAsync(Message message);
        Task SetMessageCheckedAsync(int id);

        Dictionary<string, int> GetNewMessagesCount(string EMail);
    }
}
