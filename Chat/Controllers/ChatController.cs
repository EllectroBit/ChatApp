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
        private IIdentity identity;

        public ChatController(IChat chatDB, IUser userDB, IIdentity identity)
        {
            db = chatDB;
            udb = userDB;
            this.identity = identity;
        }

        public IActionResult Index()
        {
            List<User> users = udb.GetUsersAsync().Result.Where(x => x.EMail != identity.GetCurrentUserName(HttpContext)).ToList();
            ViewData["NewMessages"] = db.GetNewMessagesCount(identity.GetCurrentUserName(HttpContext));
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> OpenChat(int UserId)
        {
            string SecondUser = udb.GetUserAsync(UserId).Result.EMail;
            ChatHub chat = await db.GetChatAsync(identity.GetCurrentUserName(HttpContext), SecondUser);
            if(chat == null)
            {
                chat = await db.GetChatAsync(SecondUser, identity.GetCurrentUserName(HttpContext));
                if (chat == null)
                    chat = await db.AddChatAsync(new ChatHub() { FirstUser = identity.GetCurrentUserName(HttpContext), SecondUser = SecondUser });
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
                Sender = udb.GetUsersAsync().Result.Where(x => x.EMail == identity.GetCurrentUserName(HttpContext)).FirstOrDefault(),
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
                if (!m.Checked && m.Sender.EMail != identity.GetCurrentUserName(HttpContext))
                    await db.SetMessageCheckedAsync(m.ID);
            }
            return View("Update" , await db.GetMessagesAsync(id));
        }
    }
}
