using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace ReservationApp.Models
{
    public partial class ReservationContext
    {
        public static async Task InitializeAsync(ReservationContext context, bool postgreSql = false)
        {
            await context.Database.MigrateAsync();
            _ = context.Model;

            #if DEBUG
                context.ClearAllData(postgreSql);
            #endif

            await context.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            try
            {
                var result = base.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                #if DEBUG
                //PrintData();
                #endif
            }
        }

        public void PrintData()
        {
            string info = Users.Count() + " " + Orders.Count() + " " + Reservations.Count() + " " + ReservationTimes.Count();
            Debug.WriteLine(info);
        }

        public void ClearAllData(bool postgreSql = false)
        {
            if (postgreSql)
            {
                //Command of PostgreSql to truncate all tables.
                string cmd = $"TRUNCATE TABLE \"Users\", \"Orders\", \"Reservations\", \"ReservationTimes\"";
                this.Database.ExecuteSqlRaw(cmd);
            }
            else
            {
                //Command of Sql Server to truncate all tables.
                if (Users.Count() > 0)
                    deleteTable("Users");
                if (Orders.Count() > 0)
                    deleteTable("Orders");
                if (Reservations.Count() > 0)
                    deleteTable("Reservations");
                if (ReservationTimes.Count() > 0)
                    deleteTable("ReservationTimes");
            }
        }

        protected void deleteTable(string tableName)
        {
            string cmd = $"TRUNCATE TABLE {tableName} CASCADE";
            this.Database.ExecuteSqlRaw(cmd);
        }
    }
}
