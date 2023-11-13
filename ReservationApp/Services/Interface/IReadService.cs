using ReservationApp.Models;
using ReservationApp.Models.Database;
using ReservationApp.Models.Database.Entities;
using ReservationApp.Models.Entities;

namespace ReservationApp.Services.Interface
{
    public interface IReadService
    {
        public abstract List<Reservation> GetReservationsByDate(DateTime start, DateTime end);
        public abstract Task<List<Reservation>> GetReservationsByDateAsync(DateTime start, DateTime end);



        public abstract Order? GetOrderByOrderId(string orderId);

        public abstract List<Order>? GetOrderList(string searchConditions);
    }
}
