using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Security.Cryptography;
using ReservationApp.Models;
using ReservationApp.Models.Entities;

namespace ReservationApp.Extensions
{
    public static class DatabaseExtensions
    {
        public static Order? GetById(this IQueryable<Order> orders, string? id, 
                                        bool tracking = false, 
                                        bool loadReference = true)
        {
            if (id == null) return null;

            Order? odata = null;
            
            if(!tracking)
            {
                orders = orders.AsNoTracking();
            }
            if(loadReference)
            {
                odata = orders.Where(o => o.Id == id).Include(o => o.User)
                              .Include(o => o.Reservations)
                              .ThenInclude(r => r.Times).FirstOrDefault();
            }
            else
            {
                odata = orders.Where(o => o.Id == id).FirstOrDefault();
            }
      
            return odata;
        }

        public static User? GetByName(this IQueryable<User> users, string? name,
                                        bool tracking = false,
                                        bool loadReference = true)
        {
            if (name == null)
                return null;
            User? user = null;

            if (!tracking)
            {
                users = users.AsNoTracking();
            }
            if (loadReference)
            {
                user = users.Where(u => u.Name == name).Include(u => u.Orders)
                              .ThenInclude(o => o.Reservations)
                              .ThenInclude(r => r.Times).FirstOrDefault();
            }
            else
            {
                user = users.Where(u => u.Name == name).FirstOrDefault();
            }

            return user;
        }

      

        public static List<Order> SortByDateAndTime(this List<Order> orders)
        {
            foreach (var order in orders)
            {
                foreach (var rdata in order.Reservations)
                {
                    rdata.Times = rdata.Times.OrderBy(o => o.StartTime).ToList();
                }
                order.Reservations = order.Reservations.OrderBy(r => r.Date.Date).ToList();
            }
            return orders.OrderBy(o => o.Reservations.First().Date.Date)
                         .ThenBy(o => o.Reservations.First().Times.First().StartTime).ToList();
        }
    }
}
