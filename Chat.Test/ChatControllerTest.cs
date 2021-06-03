using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Chat.Models;
using Microsoft.Data.SqlClient;
using Chat.Controllers;
using Microsoft.AspNetCore.Mvc;
using Chat.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Chat.Test
{
    public class ChatControllerTest
    {
        [Fact]
        public void IndexReturnViewResult()
        {
            //Arrange
            var UserMock = new Mock<IUser>();
            var ChatMock = new Mock<IChat>();
            var IdentityMock = new Mock<IIdentity>();
            UserMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(new List<User>());
            var controller = new ChatController(ChatMock.Object, UserMock.Object, IdentityMock.Object);
            //Act
            var model = controller.Index();
            //Assert
            Assert.IsType<ViewResult>(model);
        }

        [Fact]
        public async Task OpenChatGetsChatAndReturnsView()
        {
            //Arrange
            var UserMock = new Mock<IUser>();
            var ChatMock = new Mock<IChat>();
            var IdentityMock = new Mock<IIdentity>();

            User user = new User { ID = 1 };
            ChatHub chat = new ChatHub();
            UserMock.Setup(x => x.GetUserAsync(user.ID)).ReturnsAsync(user);
            ChatMock.Setup(x => x.GetChatAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(chat);

            var controller = new ChatController(ChatMock.Object, UserMock.Object, IdentityMock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.OpenChat(user.ID);
            //Assert
            Assert.Equal("Chat", result.ViewName);
            Assert.Equal(chat, (ChatHub)result.Model);
        }

        [Fact]
        public async Task OpenChatAddsNewChatAndReturnsView()
        {
            //Arrange
            var UserMock = new Mock<IUser>();
            var ChatMock = new Mock<IChat>();
            var IdentityMock = new Mock<IIdentity>();

            User user = new User { ID = 1 };
            ChatHub chat = null;
            UserMock.Setup(x => x.GetUserAsync(user.ID)).ReturnsAsync(user);
            ChatMock.Setup(x => x.GetChatAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(chat);

            var controller = new ChatController(ChatMock.Object, UserMock.Object, IdentityMock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.OpenChat(user.ID);
            //Assert
            Assert.Equal("Chat", result.ViewName);
            ChatMock.Verify(x => x.AddChatAsync(It.IsAny<ChatHub>()));
        }

        [Fact]
        public async Task SendMessageCreatesNewMessageInDbAndRedirectsToOpenChat()
        {
            //Arrange
            var UserMock = new Mock<IUser>();
            var ChatMock = new Mock<IChat>();
            var IdentityMock = new Mock<IIdentity>();

            UserMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(new List<User>() { new User() { EMail = "email@mail.com" , ID = 1} });
            IdentityMock.Setup(x => x.GetCurrentUserName(It.IsAny<Microsoft.AspNetCore.Http.HttpContext>())).Returns("email@mail.com");
            var controller = new ChatController(ChatMock.Object, UserMock.Object, IdentityMock.Object);
            //Act
            RedirectToActionResult result = (RedirectToActionResult)await controller.SendMessage("message", 1, 1);
            //Assert
            Assert.Equal("OpenChat", result.ActionName);
            ChatMock.Verify(x => x.AddMessageAsync(It.Is<Message>(x => x.Text == "message")));
        }

        [Fact]
        public async Task UpdateMakesNewMessagesChekedAndReturnsViewWithModelOfMessagesList()
        {
            //Arrange
            var UserMock = new Mock<IUser>();
            var ChatMock = new Mock<IChat>();
            var IdentityMock = new Mock<IIdentity>();

            List<Message> messages = new List<Message>() { new Message() { ID = 1, Sender = new User() { EMail = "email@mail.com" }, Checked = false } };
            ChatMock.Setup(x => x.GetMessagesAsync(1)).ReturnsAsync(messages);
            IdentityMock.Setup(x => x.GetCurrentUserName(It.IsAny<Microsoft.AspNetCore.Http.HttpContext>())).Returns("admin@mail.com");
            var controller = new ChatController(ChatMock.Object, UserMock.Object, IdentityMock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.Update(1);
            //Assert
            Assert.Equal("Update", result.ViewName);
            Assert.Equal(messages, result.Model);
            ChatMock.Verify(x => x.SetMessageCheckedAsync(1));
        }
    }
}
