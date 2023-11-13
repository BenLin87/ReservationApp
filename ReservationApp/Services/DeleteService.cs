using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ReservationApp.Extensions;
using ReservationApp.Models;
using ReservationApp.Models.Database;
using ReservationApp.Models.Entities;
using ReservationApp.Services.Interface;

namespace ReservationApp.Services
{
    public class DeleteService : BasicService, IDeleteService
    {
        private readonly IStringLocalizer<DeleteService> _localizer;

        public DeleteService(IServiceProvider provider) : base(provider) {
            this._localizer = provider.GetRequiredService<IStringLocalizer<DeleteService>>();
        }

        public bool DeleteOrder(string jsonData)
        {
            int result = 0;
            string orderId = jsonData;
            
            using(var transaction = _context.Database.BeginTransaction()) 
            { 
                try
                { 
                    Order? odata = _context.Orders.GetById(orderId, true);
                    if (odata != null)
                    {
                        _context.Remove(odata);
                        result = _context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception(_localizer["InvalidOrderId"]);
                    }
                    transaction.Commit();
                }
                catch (Exception )
                {
                    transaction.Rollback();
                    throw;
                }
            }
            if (result < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
