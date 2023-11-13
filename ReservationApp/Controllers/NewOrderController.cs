using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NuGet.Protocol;
using ReservationApp.Extensions;
using ReservationApp.Resources;
using ReservationApp.Services;
using ReservationApp.Services.Interface;

namespace ReservationApp.Controllers
{
    public class NewOrderController : Controller
    {
        private ICreateService _createService;
        private readonly IEncryptService _encryptService;
        private IReadService _readService;
        private readonly IStringLocalizer<NewOrderController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public NewOrderController(IServiceProvider provider) 
        {
            this._localizer = provider.GetRequiredService<IStringLocalizer<NewOrderController>>();
            this._sharedLocalizer = provider.GetRequiredService<IStringLocalizer<SharedResource>>();
            this._readService = provider.GetRequiredService<IReadService>();
            this._createService = provider.GetRequiredService<ICreateService>();
            this._encryptService = provider.GetRequiredService<IEncryptService>();
        }

        public IActionResult Index()
        {
            //return View("~/Views/PartialViews/_TimeTable_rwd.cshtml");
            try
            {
                DateTime startDate = DateTime.Now.AddDays(-1);
                var rdatas = _readService.GetReservationsByDate(startDate, startDate.AddDays(7));
                if (rdatas == null || rdatas.Count() == 0)
                    ViewBag.ExistedReservations = null;
                else
                {
                    var test = _encryptService.Encrypt(rdatas.ToJson());
                    ViewBag.ExistedReservations = test;
                }
                ViewBag.NextUrl = "/NewOrder/InputUserData";
                ViewBag.StartDate = startDate.Date;
                return View("~/Views/Common/SelectReservationTime.cshtml");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500; 
                return Json(ex.Message);
            }
        }

      

        public IActionResult AddNewOrder(string jsonData)
        {
            bool result = false;
            string message = string.Empty;
            try
            {
                var decrypted = _encryptService.Decrypt(jsonData);
                result = _createService.AddNewOrder(decrypted, out string orderId);
                //message = "訂單建立完成!" + Environment.NewLine + "訂單編號 : " + orderId;
                //message = _localizer["AddOrderDone"] + "!" + "\n" + _sharedLocalizer["OrderId"] + " : " + orderId;
                message = orderId;
            }
            catch(Exception ex)
            {
                result = false;
                //message = "訂單建立失敗!";
                /*
                message = _localizer["AddOrderFail"] + "!";
                if (ex.Message != string.Empty)
                    message += Environment.NewLine + ex.Message;*/
                message = ex.Message;
            }
            //If add successed, message = orderId
            //If add failed, message = reason of failure
            var response = new { 
                result, 
                message
            };
            return Json(response.ToJson());
        }

        public IActionResult InputUserData()
        {
            return View();
        }

        public IActionResult ConfirmOrder()
        {
            ViewBag.ReturnUrl = "/NewOrder/AddNewOrder";
            //ViewBag.Message = "建立訂單?";
            ViewBag.Message = _localizer["AddOrder"] + "?";
            ViewBag.ActionName = "AddOrder";
            return View("~/Views/Common/ConfirmOrder.cshtml");
        }
    }
}
