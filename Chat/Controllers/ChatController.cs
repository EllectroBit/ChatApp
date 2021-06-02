using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Infrustructure;
using Microsoft.AspNetCore.Mvc;
using Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Chat.Interfaces;
using Microsoft.AspNetCore;

namespace Chat.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private IChat db;
        private IUser udb;

        public ChatController(IChat chatDB, IUser userDB)
        {
            db = chatDB;
            udb = userDB;
        }

        public IActionResult Index()
        {
            List<User> users = udb.GetUsersAsync().Result.Where(x => x.EMail != User.Identity.Name).ToList();
            ViewData["NewMessages"] = db.GetNewMessagesCount(User.Identity.Name);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> OpenChat(int UserId)
        {
            string SecondUser = udb.GetUserAsync(UserId).Result.EMail;
            ChatHub chat = await db.GetChatAsync(User.Identity.Name, SecondUser);
            if(chat == null)
            {
                chat = await db.GetChatAsync(SecondUser, User.Identity.Name);
                if (chat == null)
                    chat = await db.AddChatAsync(new ChatHub() { FirstUser = User.Identity.Name, SecondUser = SecondUser });
            }

            ViewData["UserID"] = UserId;
            return View("Chat", chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, int id, int UserID)
        {
            await db.AddMessageAsync(new Message()
            {
                Text = message,
                DateTime = DateTime.Now,
                Sender = udb.GetUsersAsync().Result.Where(x => x.EMail == User.Identity.Name).FirstOrDefault(),
                ChatHubID = id
            });
            return RedirectToAction("OpenChat", new { UserID = UserID });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            List<Message> messages = await db.GetMessagesAsync(id);
            foreach(var m in messages)
            {
                if (!m.Checked && m.Sender.EMail != User.Identity.Name)
                    await db.SetMessageCheckedAsync(m.ID);
            }
            return View(await db.GetMessagesAsync(id));
        }
    }
}
