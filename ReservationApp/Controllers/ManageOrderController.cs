using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ReservationApp.Extensions;
using ReservationApp.Models.Entities;
using ReservationApp.Resources;
using ReservationApp.Services.Interface;

namespace ReservationApp.Controllers
{
    public class ManageOrderController : Controller
    {
        private IReadService _readService;
        private IUpdateService _updateService;
        private IDeleteService _deleteService;
        private readonly IEncryptService _encryptService;
        private readonly IStringLocalizer<ManageOrderController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public ManageOrderController(IServiceProvider provider)
        {
            this._localizer = provider.GetRequiredService<IStringLocalizer<ManageOrderController>>();
            this._sharedLocalizer = provider.GetRequiredService<IStringLocalizer<SharedResource>>();
            this._readService = provider.GetRequiredService<IReadService>();
            this._updateService = provider.GetRequiredService<IUpdateService>();
            this._deleteService = provider.GetRequiredService<IDeleteService>();
            this._encryptService = provider.GetRequiredService<IEncryptService>();
        }

        public IActionResult ManageOrder()
        {
            ViewBag.Controller = "/ManageOrder";
            return View();
        }

        [Authorize]
        public IActionResult ModifyReservationTime(string orderId)
        {
            try
            {
                Order? order = _readService.GetOrderByOrderId(orderId);
                if (order != null)
                {
                    ViewBag.OrderData = _encryptService.Encrypt(order.ToJson());
                    DateTime startDate = DateTime.Now.Date;
                    if(order.Reservations != null && order.Reservations.Count > 0)
                    {
                        startDate = order.Reservations.Min(r => r.Date).Date;

                        //If the start date exceeds the current date,
                        //it will allow selecting reservation date/time beyond 7 days from now.
                        startDate = startDate > DateTime.Now.Date ? DateTime.Now.Date : startDate;
                    }
                    var existedReservations = _readService.GetReservationsByDate(startDate, startDate.AddDays(7));
                    
                    ViewBag.ExistedReservations = _encryptService.Encrypt(existedReservations.ToJson());

                    ViewBag.NextUrl = "/ManageOrder/ConfirmModifyOrder";
                    ViewBag.StartDate = startDate;
                    return View("~/Views/Common/SelectReservationTime.cshtml");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
           
            return Json(false);
        }

        [Authorize]
        public IActionResult ConfirmModifyOrder(string orderId)
        {
            ViewBag.ReturnUrl = "/ManageOrder/ModifyOrder";
            ViewBag.Message = _localizer["ModifyOrder"] + "?"; //"編輯訂單?";
            ViewBag.ActionName = "ModifyOrder";
            return View("~/Views/Common/ConfirmOrder.cshtml");
        }

        [Authorize]
        public IActionResult ConfirmDeleteOrder(string orderId)
        {
            try
            {
                string orderData = string.Empty;
                Order? odata = _readService.GetOrderByOrderId(orderId);
                if (odata == null)
                {
                    //TODO 待測試
                    var message = _localizer["Invalid"] + _sharedLocalizer["OrderId"]; //"無效的訂單編號";
                    ViewBag.Message = message;
                    ViewBag.OrderData = null;
                    //Response.StatusCode = 500;
                    //return Json(message);
                    //ViewBag.Message = _localizer["Invalid"] + _sharedLocalizer["OrderId"]; //"無效的訂單編號";
                    //ViewBag.ActionName = "DeleteOrder";
                    //return View("~/Views/Common/ConfirmOrder.cshtml");
                }
                else
                {
                    ViewBag.OrderData = _encryptService.Encrypt(odata.ToJson());
                    ViewBag.ReturnUrl = "/ManageOrder/DeleteOrder";
                }
                
                //.Message = _localizer["DeleteOrder"]  + "?"; //"刪除訂單?";
                ViewBag.ActionName = "DeleteOrder";
                return View("~/Views/Common/ConfirmOrder.cshtml");
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        public IActionResult GetOrderList(string searchConditions)
        {
            try
            {
                var orders = _readService.GetOrderList(searchConditions);
                if (orders == null)
                {
                    orders = new List<Order>();
                }
                return PartialView("~/Views/PartialViews/_OrderListContent.cshtml", orders);
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(_localizer["SearchFailed"] + "!" + Environment.NewLine + ex.Message);
            }
        }

        [Authorize]
        public IActionResult ModifyOrder(string jsonData)
        {
            bool result = false;
            string message = string.Empty;
            try
            {
                result = _updateService.ModifyOrder(_encryptService.Decrypt(jsonData));
                message = "";// _localizer["ModifyOrderDone"]; //"訂單修改完成";
            }
            catch (Exception ex)
            {
                result = false;
                /*
                message = _localizer["ModifyOrderFail"]; //"訂單修改失敗!";
                if (ex.Message != string.Empty)
                    message += Environment.NewLine + ex.Message;*/
                message = ex.Message;
            }
            //If modify failed, message = reason of failure
            var response = new {
                result,
                message
            };
            return Json(response.ToJson());
        }

        [Authorize]
        public IActionResult DeleteOrder(string jsonData)
        {
            bool result = false;
            string message = string.Empty;
            try {
                var decrypted = _encryptService.Decrypt(jsonData);
                result = _deleteService.DeleteOrder(decrypted);
                message = "";// _localizer["DeletOrderDone"]; //"訂單刪除完成";
            }
            catch (Exception ex)
            {
                result = false;
                /*
                message = _localizer["DeletOrderFail"]; //"訂單刪除失敗!";
                
                if (ex.Message != string.Empty)
                    message += Environment.NewLine + ex.Message;
                */
                message = ex.Message;
            }
            //If delete failed, message = reason of failure
            var response = new { 
                result, 
                message
            };
            return Json(response.ToJson());
        }
    }
}
