using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReservationApp.Models;
using ReservationApp.Services.Interface;
using System.Security.Claims;

namespace ReservationApp.Services
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<LoginService> _localizer;

        public LoginService(IHttpContextAccessor contextAccessor, IServiceProvider provider)
        {
            this._contextAccessor = contextAccessor;
            this._localizer = provider.GetRequiredService<IStringLocalizer<LoginService>>();
        }

        public bool Logout()
        {
            _contextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }

        public bool Login(string name, string password)
        {
            if (name == "admin" && password == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim("FullName", name)
                };

                var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                _contextAccessor.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                
                return true;
            }
            else
            {
                throw new Exception(_localizer["WrongAcctOrPwd"] + "!");//管理者帳號或密碼錯誤!");
            }
        }

        public bool Login(LoginModel? loginData)
        {
            if (loginData == null)
            {
                return false;
            }
            return Login(loginData.name, loginData.password);
        }

        public string? GetCurrentUserName()
        {
            return _contextAccessor.HttpContext?.User.Identity?.Name;
        }
    }
}
