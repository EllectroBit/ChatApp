using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Infrustructure;
using Microsoft.AspNetCore.Mvc;
using Chat.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Chat.Interfaces;

namespace Chat.Controllers
{
    public class AccountController : Controller
    {
        private IUser db;
        private IIdentity identity;

        public AccountController(IUser db, IIdentity identity)
        {
            this.db = db;
            this.identity = identity;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                User user = await db.GetUserAsync(login);
                if (user != null)
                {
                    await identity.Authenticate(user, HttpContext);//Authenticate(user);
                    return RedirectToAction("Index", "Chat");
                }
                else
                    ModelState.AddModelError("", "Wrong Email or Password");
            }
            return View("Index");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<User> users = await db.GetUsersAsync();
                User user = users.FirstOrDefault(u => u.EMail == model.EMail);
                if (user == null)
                {
                    user = new User { EMail = model.EMail, Password = model.Password };
                    await db.AddUserAsync(user);

                    await identity.Authenticate(user, HttpContext);//Authenticate(user);

                    return RedirectToAction("Index", "Chat");
                }
                else
                    ModelState.AddModelError("", "Wrong Email or Password");
            }
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await identity.LogOut(HttpContext);
            return RedirectToAction("Index");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckEMailAsync(string email)
        {
            List<User> users = await db.GetUsersAsync();
            return Json(!users.Any(x => x.EMail == email));
        }

        //private async Task Authenticate(User user)
        //{
        //    List<Claim> claims = new List<Claim>()
        //    {
        //        new Claim(ClaimsIdentity.DefaultNameClaimType, user.EMail)
        //    };

        //    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        //}
    }
}