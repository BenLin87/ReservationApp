using Microsoft.EntityFrameworkCore;
using ReservationApp.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ReservationApp.Models.Database.Entities
{
    public class Reservation
    {
        [Key]
        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.Empty;

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [JsonIgnore]
        public DateTime Date { get; set; }

        [JsonPropertyName("date")]
        [NotMapped]
        public string DateString
        {
            get
            {
                //For comparing to the date from views, since they only use "MM/dd" right now.
                return Date.ToString("yyyy/MM/dd");
            }
            set
            {
                //Try to parse the datetime by format "MM/dd" or "yyyy/MM/dd".
                DateTime inputDt = DateTime.Now;
                bool result = DateTime.TryParseExact(value, "MM/dd", null, DateTimeStyles.None, out inputDt);
                if (result)
                    Date = inputDt;
                else
                {
                    result = DateTime.TryParseExact(value, "yyyy/MM/dd", null, DateTimeStyles.None, out inputDt);
                    Date = result? inputDt: Date;
                }
            }
        }

        [JsonIgnore]
        public Guid OrderGuid { get; set; }

        [JsonIgnore]
        public Order Order { get; set; } = null!;

        [JsonPropertyName("times")]
        public ICollection<ReservationTime> Times { get; set; } = new List<ReservationTime>();
    }
}
