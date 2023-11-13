using Microsoft.EntityFrameworkCore;
using ReservationApp.Models;

namespace ReservationApp.Services
{
    public class BasicService
    {
        protected ReservationContext _context;

        public BasicService(IServiceProvider provider)
        {
            _context = provider.GetRequiredService<ReservationContext>();
        }
    }
}
