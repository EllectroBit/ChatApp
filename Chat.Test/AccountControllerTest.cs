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
    public class AccountControllerTest
    {
        [Fact]
        public void IndexReturnViewResult()
        {
            //Arrange
            var mock = new Mock<IUser>();
            var controller = new AccountController(mock.Object);
            //Act
            var model = controller.Index();
            //Assert
            Assert.IsType<ViewResult>(model);
        }

        [Fact]
        public void RegisterReturnViewResult()
        {
            //Arrange
            var mock = new Mock<IUser>();
            var controller = new AccountController(mock.Object);
            //Act
            var model = controller.Register();
            //Assert
            Assert.IsType<ViewResult>(model);
        }

        [Fact]
        public async Task LoginReturnsIndexViewResultAsync()
        {
            //Arrange
            LoginViewModel login = new LoginViewModel();

            var mock = new Mock<IUser>();
            var controller = new AccountController(mock.Object);
            controller.ModelState.AddModelError("Email", "Wrong Email");
            //Act
            ViewResult result = (ViewResult)await controller.Login(login);
            //Assert
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public async Task LoginReturnsIndexViewResultAndAddModelErorrAsync()
        {
            //Arrange
            LoginViewModel login = new LoginViewModel();

            var mock = new Mock<IUser>();
            User user = null;
            mock.Setup(m => m.GetUserAsync(login)).ReturnsAsync(user);
            var controller = new AccountController(mock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.Login(login);
            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public async Task LoginAuthenticateAndRedirectsToStoreControllerIndexAsync()
        {
            //Arrange
            LoginViewModel login = new LoginViewModel();
            User user = new User() { ID = 1, EMail = "email@mail.com", Password = "1111" };

            var mock = new Mock<IUser>();
            mock.Setup(m => m.GetUserAsync(login)).ReturnsAsync(user);
            var controller = new AccountController(mock.Object);
            //Act
            RedirectToActionResult result = (RedirectToActionResult)await controller.Login(login);
            //Assert
            Assert.True(controller.ModelState.IsValid);
            Assert.Equal("Chat", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
            mock.Verify(x => x.Authenticate(user, controller.HttpContext));
        }

        [Fact]
        public async Task RegisterReturnsIndexViewResultAsync()
        {
            //Arrange
            RegisterViewModel reg = new RegisterViewModel();

            var mock = new Mock<IUser>();
            var controller = new AccountController(mock.Object);
            controller.ModelState.AddModelError("Email", "Wrong Email");
            //Act
            ViewResult result = (ViewResult)await controller.Register(reg);
            //Assert
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public async Task RegisterReturnsIndexViewResultAndAddModelErorrAsync()
        {
            //Arrange
            RegisterViewModel reg = new RegisterViewModel() { EMail = "email@mail.com" };

            var mock = new Mock<IUser>();
            List<User> users = new List<User>() { new User() { EMail = "email@mail.com", ID = 1, Password = "1111" } };
            mock.Setup(m => m.GetUsersAsync()).ReturnsAsync(users);
            var controller = new AccountController(mock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.Register(reg);
            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(1, controller.ModelState.ErrorCount);
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public async Task RegisterAuthenticatesUserAndAddsToDbAdnRedirectsToIndexChatAsync()
        {
            //Arrange
            RegisterViewModel reg = new RegisterViewModel() { EMail = "email@mail.com", Password = "1111", PasswordConfirm = "1111" };
            User user = new User() { EMail = reg.EMail, Password = reg.Password};

            var mock = new Mock<IUser>();
            List<User> users = new List<User>();
            mock.Setup(m => m.GetUsersAsync()).ReturnsAsync(users);
            mock.Setup(m => m.AddUserAsync(It.IsAny<User>()));
            var controller = new AccountController(mock.Object);
            //Act
            RedirectToActionResult result = (RedirectToActionResult)await controller.Register(reg);
            //Assert
            Assert.True(controller.ModelState.IsValid);
            mock.Verify(x => x.AddUserAsync(It.IsAny<User>()));
            mock.Verify(x => x.Authenticate(It.IsAny<User>(), controller.HttpContext));
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task CheckEmailReturnsJsonWithTrueResultAsync()
        {
            //Arrange
            List<User> users = new List<User>() { new User() { EMail = "email@mail.com" } };

            var mock = new Mock<IUser>();
            mock.Setup(x => x.GetUsersAsync()).ReturnsAsync(users);
            var controller = new AccountController(mock.Object);
            //Act
            JsonResult result = (JsonResult)await controller.CheckEMailAsync("email@mail.com");
            //Assert
            Assert.Equal(false, result.Value);
        }
    }
}
