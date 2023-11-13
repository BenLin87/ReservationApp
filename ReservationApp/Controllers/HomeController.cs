using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ReservationApp.Models;
using ReservationApp.Services.Interface;
using System.Diagnostics;
using System.Globalization;

namespace ReservationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _loginService;
        int count = 0;
        public HomeController(IServiceProvider provider)
        {
            this._localizer = provider.GetRequiredService<IStringLocalizer<HomeController>>();
            this._logger = provider.GetRequiredService<ILogger<HomeController>>();
            this._loginService = provider.GetRequiredService<ILoginService>();
        }

        public IActionResult Index()
        {
            Debug.WriteLine(count.ToString());
            count++;
            //if (HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            if (_loginService.GetCurrentUserName() != null)
            {
                ViewBag.Admin = "true";
            }
            else
                ViewBag.Admin = null;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SetLanguage(string language)
        {
            try
            {
                CultureInfo[] supportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

                // 查詢是否目前環境支援特定的 CultureInfo
                bool isSupported = supportedCultures.Any(c => c.Name.Equals(language, StringComparison.OrdinalIgnoreCase));
                if (language == "zh-TW" && !isSupported)
                    language = "zh-Hant";

                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
                return Json(true);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
            return Json(true);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}