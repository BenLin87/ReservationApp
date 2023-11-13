using ReservationApp.Models;

namespace ReservationApp.Services.Interface
{
    public interface ICreateService
    {
        public abstract bool AddNewOrder(string jsonData, out string message);
    }
}
