using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ReservationApp.Controllers;
using ReservationApp.Extensions;
using ReservationApp.Models.Database.Entities;
using ReservationApp.Models.Entities;
using ReservationApp.Services.Interface;
using System.Security.Cryptography;

namespace ReservationApp.Services
{
    public class CreateService : BasicService, ICreateService
    {
        private IStringLocalizer<CreateService> _localizer;

        public CreateService(IServiceProvider provider) :base(provider) {
            this._localizer = provider.GetRequiredService<IStringLocalizer<CreateService>>();
        }

        public string generateRandomId_v1(int length)
        {
            var rand = RandomNumberGenerator.Create();
            var bits = (length * 6);
            var byte_size = ((bits + 7) / 8);
            var bytesarray = new byte[byte_size];
            rand.GetBytes(bytesarray);
            return Convert.ToBase64String(bytesarray);
        }

        private string generateRandomId_v2(int length)
        {
            string str = Guid.NewGuid().ToString().Replace("-", "");
            return str.Substring(0, 8);
        }

        public bool AddNewOrder(string jsonData, out string message)
        {
            var order = jsonData.ToObject<Order>();
            message = string.Empty;
            if (order == null)
            {
                throw new Exception("Json Deserialize failed.");
            }
            if(order.User.Name.Length > 50)
            {
                //throw new Exception("預約人姓名長度應在50個字元以內.");
                throw new Exception(_localizer["UserNameMaxLengthWarning"] + ".");
            }
            order.Guid = Guid.NewGuid();
            order.Id = generateRandomId_v2(8);

            foreach (Reservation rdata in order.Reservations)
            {
                if (rdata.Guid == Guid.Empty)
                {
                    rdata.Guid = Guid.NewGuid();
                }
                rdata.OrderGuid = order.Guid;
                foreach (ReservationTime tdata in rdata.Times)
                {
                    if (tdata.Guid == Guid.Empty)
                    {
                        tdata.Guid = Guid.NewGuid();
                    }
                    tdata.ReservationGuid = rdata.Guid;

                    //Check the date in Reservation and ReservationTime are same.
                    if(tdata.StartTime.Date != rdata.Date.Date)
                    {
                        tdata.StartTime = rdata.Date.Date + tdata.StartTime.TimeOfDay;
                    }
                    if (tdata.EndTime.Date != rdata.Date.Date)
                    {
                        tdata.EndTime = rdata.Date.Date + tdata.EndTime.TimeOfDay;
                    }
                }
            }
            
            var udata = _context.Users.Where(u => u.Name == order.UserName).Include(u => u.Orders).FirstOrDefault();
            using(var transactio = _context.Database.BeginTransaction()) {
                try
                {
                    if (udata == null)
                    {
                        udata = new User();
                        udata.Guid = Guid.NewGuid();
                        udata.Name = order.UserName;
                        udata.Orders.Add(order);
                        _context.Users.Add(udata);
                    }
                    else
                    {
                        order.User = udata;
                        _context.Orders.Add(order);
                    }
                    _context.SaveChanges();
                    transactio.Commit();
                }
                catch (Exception)
                {
                    transactio.Rollback();
                    throw;
                }
            }
            message = order.Id;
            return true;
        }
    }
}
