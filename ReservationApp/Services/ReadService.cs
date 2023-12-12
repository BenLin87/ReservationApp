using LinqKit;
using Microsoft.EntityFrameworkCore;
using ReservationApp.Extensions;
using ReservationApp.Models;
using ReservationApp.Models.Database.Entities;
using ReservationApp.Models.Entities;
using ReservationApp.Services.Interface;
using System;
using System.Diagnostics;
using static ReservationApp.Models.SearchCondition_Order;

namespace ReservationApp.Services
{
    public class ReadService : BasicService, IReadService
    {
        public ReadService(IServiceProvider provider) : base(provider) { }

        public async Task<List<Reservation>> GetReservationsByDateAsync(DateTime start, DateTime end)
        {
            if (_context.Reservations.Count() > 0)
            {
                 var rdatas = await _context.Reservations
                    .Where(r => r.Date.Date >= start.Date && r.Date.Date <= end.Date)
                    .Include(r => r.Times).AsNoTracking().ToListAsync().ConfigureAwait(false);
                return rdatas;
            }
            return new List<Reservation>();
        }

        public List<Reservation> GetReservationsByDate(DateTime start, DateTime end)
        {
            List<Reservation> rdatas = new List<Reservation>();

            if (_context.Reservations.Count() > 0)
            {
                rdatas = _context.Reservations
                    .Where(r => r.Date.Date >= start.Date && r.Date.Date <= end.Date)
                    .Include(r => r.Times).AsNoTracking().ToList();
            }
            
            return rdatas;
        }

        public Order? GetOrderByOrderId(string orderId)
        {
            Order? order = null;
            
            order = _context.Orders.GetById(orderId);
            
            return order;
        }

        
        private ICollection<Order> filterDateFrom(ICollection<Order> source, DateTime from, bool filterTime)
        {
            IEnumerable<dynamic> result;
            if (filterTime)
            {
                result = source.Select(o => new
                   {
                       OrderGuid = o.Guid,
                       MaxEndTime = o.Reservations.SelectMany(r => r.Times).Max(t => t.EndTime)
                   })
                   .Where(o => o.MaxEndTime > from)
                   .ToList();
            }
            else
            {
                from = from.AddDays(-1).Date;
                result = source.Select(o => new
                {
                    OrderGuid = o.Guid,
                    MaxDate = o.Reservations.Max(r => r.Date)
                }).Where(o => o.MaxDate > from)
                   .ToList(); ;
            }
            var orderList = result.Join(
                  _context.Orders,
                  r => r.OrderGuid,
                  o => o.Guid,
                  (r, o) => o
              ).ToList();
            return orderList;
        }

        private void addDateTimeLinq(
            ref ExpressionStarter<Order> predicate,
            DateTime? from, DateTime? to,
            bool compareFromTime = false, bool compareToTime = false)
        {
            if (compareFromTime)
            {
                if (from == null || from < DateTime.Now)
                    from = DateTime.Now;

                var fromDate = from?.Date;
                var fromTime = from?.TimeOfDay;

                //TimeSpan is not supported by EF Core.
                predicate = predicate.And(
                     o => o.Reservations.Any(r => r.Date.Date > fromDate ||
                     (r.Date.Date == fromDate && (r.Times.Max(t => t.EndTime) > from))));
                /*
                predicate = predicate.And(
                    o => o.Reservations.Any(r => r.Date.Date > fromDate) ||
                    o.Reservations.Any(
                        r => r.Date.Date == fromDate &&
                        ((r.Times.Max(t => t.EndTime) - r.Date.Date) > fromTime)));*/
            }
            else
            {
                if (from != null)
                {
                    var fromDate = from?.Date;
                    predicate = predicate.And(o => o.Reservations.Any(r => r.Date.Date >= fromDate));
                }
            }
            if (compareToTime)
            {
                if (to == null || to > DateTime.Now)
                    to = DateTime.Now;

                var toDate = to?.Date;
                var toTime = to?.TimeOfDay;

                //TimeSpan is not supported by EF Core.
                predicate = predicate.And(
                     o => o.Reservations.Any(r => r.Date.Date < toDate ||
                     (r.Date.Date == toDate && (r.Times.Max(t => t.EndTime) < to))));
                /*
                predicate = predicate.And(
                    o => o.Reservations.Max(r => r.Date.Date) < toDate ||
                    o.Reservations.Any(
                        r => r.Date.Date == toDate &&
                        ((r.Times.Max(t => t.EndTime) - r.Date.Date) <= toTime)));*/
            }
            else
            {
                if (to != null)
                {
                    var toDate = to?.Date;
                    predicate = predicate.And(o => o.Reservations.Any(r => r.Date.Date <= toDate));
                }
            }
        }


        public List<Order>? GetOrderList(string searchConditions)
        {
            List<Order>? result = null;
            var conditions = searchConditions.ToObject<SearchCondition_Order>();
            if (conditions != null)
            {
                var predicate = PredicateBuilder.New<Order>(true);
                
                if (!string.IsNullOrEmpty(conditions.OrderId))
                {
                    predicate = predicate.And(o => o.Id == conditions.OrderId);
                }
                if (!string.IsNullOrEmpty(conditions.UserName))
                {
                    predicate = predicate.And(o => o.User.Name == conditions.UserName);
                }
               
                DateTime? dateFrom = null;
                DateTime? dateTo = null;
                if (!string.IsNullOrEmpty(conditions.DateFrom))
                {
                    dateFrom = DateTime.Parse(conditions.DateFrom).Date;
                    predicate = predicate.And(o => o.Reservations.Any(r => r.Date.Date >= dateFrom));
                }
                if (!string.IsNullOrEmpty(conditions.DateTo))
                {
                    dateTo = DateTime.Parse(conditions.DateTo).Date;
                    predicate = predicate.And(o => o.Reservations.Any(r => r.Date.Date <= dateTo));
                }

                result = _context.Orders.AsExpandable().Where(predicate)?
                    .Include(o => o.User).Include(o => o.Reservations).ThenInclude(r => r.Times)
                    .ToList();

                result = result?.SortByDateAndTime();
                if (result == null) return null;

                if (conditions.DisplayMode == DisplayModes.Avaliable)
                {
                    DateTime timeFrom = DateTime.Now;
                    result = result.Where(o => o.Reservations.Last().Times.Last().EndTime > timeFrom).ToList();
                }
                else if (conditions.DisplayMode == DisplayModes.Overdue)
                {
                    DateTime timeFrom = DateTime.Now;
                    result = result.Where(o => o.Reservations.Last().Times.Last().EndTime < timeFrom).ToList();
                }
            }
            return result;
        }
    }
}
