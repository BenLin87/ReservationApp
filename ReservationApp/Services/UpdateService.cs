using ReservationApp.Extensions;
using ReservationApp.Models;
using ReservationApp.Models.Database;
using ReservationApp.Models.Database.Entities;
using ReservationApp.Models.Entities;
using ReservationApp.Models.JsonResponseStructs;
using ReservationApp.Services.Interface;

namespace ReservationApp.Services
{
    public class UpdateService : BasicService, IUpdateService
    {
        public UpdateService(IServiceProvider provider) : base(provider) { }

        public bool ModifyOrder(string jsonData)
        {
            int result = -1;
            var modifiedOrder = jsonData.ToObject<ModifiedOrderJson>();
            if (modifiedOrder == null)
            {
                throw new Exception("Order data is null.");
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Order? odata = _context.Orders.GetById(modifiedOrder.OrderId, true);
                    if (odata != null)
                    {
                        List<Reservation> addReservations = new List<Reservation>();
                        List<ReservationTime> addReservationTimes = new List<ReservationTime>();

                        for (int i = 0; i < modifiedOrder.AddReservations.Count; i++)
                        {
                            string date = modifiedOrder.AddReservations[i].DateString;
                            var rdata = odata.Reservations.Where(r => r.DateString == date).FirstOrDefault();
                            bool reservationExisted = true;
                            if (rdata == null)
                            {
                                reservationExisted = false;
                                rdata = modifiedOrder.AddReservations[i];
                                rdata.OrderGuid = odata.Guid;
                                rdata.Guid = Guid.NewGuid();
                            }
                            foreach (ReservationTime tdata in modifiedOrder.AddReservations[i].Times)
                            {
                                tdata.Guid = Guid.NewGuid();
                                tdata.ReservationGuid = rdata.Guid;
                            }

                            if (reservationExisted)
                            {
                                addReservationTimes.AddRange(modifiedOrder.AddReservations[i].Times);
                            }
                            else
                            {
                                addReservations.Add(rdata);
                            }
                        }

                        List<ReservationTime> deleteList = new List<ReservationTime>();

                        for (int j = 0; j < modifiedOrder.DeleteReservations.Count; j++)
                        {
                            Guid guid = Guid.Parse(modifiedOrder.DeleteReservations[j]);
                            ReservationTime? tdata = _context.ReservationTimes.Where(t => t.Guid == guid).FirstOrDefault();
                            if (tdata != null) deleteList.Add(tdata);
                        }
                        _context.ReservationTimes.RemoveRange(deleteList);
                        //result = _context.SaveChanges();
                        //odata.Reservations.AddRange(addList);
                        if (addReservations.Count > 0)
                        {
                            _context.Reservations.AddRange(addReservations);
                        }
                        if (addReservationTimes.Count > 0)
                        {
                            _context.ReservationTimes.AddRange(addReservationTimes);
                        }
                        result = _context.SaveChanges();

                        var deleteReservations = new List<Reservation>();
                        foreach (Reservation rdata in odata.Reservations)
                        {
                            if (rdata.Times.Count == 0)
                            {
                                deleteReservations.Add(rdata);
                            }
                        }
                        if (deleteReservations.Count > 0)
                            _context.Reservations.RemoveRange(deleteReservations);
                        if (odata.Reservations.Count == 0)
                        {
                            _context.Orders.Remove(odata);
                        }
                        result = _context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception)
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
