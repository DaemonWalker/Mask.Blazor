using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mask.Blazor.Data;
using Mask.Blazor.Models;
using Mask.Blazor.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mask.Blazor.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IMaskDatabase maskDataBase;
        public LoginModel(IMaskDatabase maskDataBase)
        {
            this.maskDataBase = maskDataBase;
        }
        public async Task<IActionResult> OnPostAsync(string account, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(account) && string.IsNullOrEmpty(password))
                {
                    return LocalRedirect("/");
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var user = await maskDataBase.Login(new AccountModel() { Account = account, Password = password });
                if (user == null)
                {
                    return LocalRedirect("/");
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account),
                    new Claim(ClaimTypes.Role, user.Level)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    RedirectUri = this.Request.Host.Value
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            }
            finally
            {

            }
            return LocalRedirect("/");

        }
    }
}
