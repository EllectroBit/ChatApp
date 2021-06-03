using Chat.Interfaces;
using Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace Chat.Infrustructure
{
    public class Identity : IIdentity
    {
        public async Task Authenticate(User user, HttpContext httpContext)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.EMail)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(httpContext, 
                CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public string GetCurrentUserName(HttpContext httpContext)
        {
            return httpContext.User.Identity.Name;
        }

        public async Task LogOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }
    }
}
