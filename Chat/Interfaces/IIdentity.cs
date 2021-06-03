using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Models;
using Microsoft.AspNetCore.Http;

namespace Chat.Interfaces
{
    public interface IIdentity
    {
        Task Authenticate(User user, HttpContext httpContext);
        Task LogOut(HttpContext httpContext);
        string GetCurrentUserName(HttpContext httpContext);
    }
}
