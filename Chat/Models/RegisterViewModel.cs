using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Wrong Email")]
        [Remote(action: "CheckEMail", controller: "Account", ErrorMessage = "Already exist")]
        public string EMail { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password mismatch")]
        [Display(Name = "Password Confirm")]
        public string PasswordConfirm { get; set; }
    }
}
