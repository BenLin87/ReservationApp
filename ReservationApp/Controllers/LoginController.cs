using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using ReservationApp.Extensions;
using ReservationApp.Models;
using ReservationApp.Resources;
using ReservationApp.Services.Interface;
using System.Diagnostics;

namespace ReservationApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IEncryptService _encryptService;
        private readonly IStringLocalizer<LoginController> _localizer;

        public LoginController(IServiceProvider provider) 
        {
            this._localizer = provider.GetRequiredService<IStringLocalizer<LoginController>>();
            this._loginService = provider.GetRequiredService<ILoginService>();
            this._encryptService = provider.GetRequiredService<IEncryptService>();
        }

        public IActionResult LoginPage()
        {
            
            return View("~/Views/Login/AdminLogin.cshtml", new LoginModel());
        }

        public IActionResult AdminLoginPage()
        {
            return View("~/Views/Login/AdminLogin.cshtml", new LoginModel());
        }

        private void encryptTest()
        {
            string test = "Test";
            string encrypted = _encryptService.Encrypt(test);
            string decrypted = _encryptService.Decrypt(encrypted);
            Debug.WriteLine(encrypted + " " + decrypted);
        }

        public IActionResult Login(LoginModel model)
        {
            if(model != null && ModelState.IsValid)
            {
                return Json(true);
            }
            return Json(false);
        }

        public IActionResult AdminLogin(string jsonData)
        {
            bool result = false;
            string message = string.Empty;
            try
            {
                string decrypted = _encryptService.Decrypt(jsonData);
                var loginData = decrypted.ToObject<LoginModel>();
                result = _loginService.Login(loginData);
                message = _localizer["AdminLoginDone"] + "!"; //"管理者登入成功!";
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
                //Response.StatusCode = 500;
            }
            var response = new
            {
                result,
                message
            };
            return Json(response.ToJson());
        }

        public IActionResult Logout()
        {
            try
            {
                if (_loginService.Logout())
                    return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(false);
        }
    }
}
