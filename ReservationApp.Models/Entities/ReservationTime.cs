using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ReservationApp.Models.Database;
using ReservationApp.Models.Database.Entities;

namespace ReservationApp.Models.Entities
{
    public class ReservationTime
    {
        [Key]
        public Guid Guid { get; set; } = Guid.Empty;

        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        [JsonIgnore]
        public DateTime StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        [JsonIgnore]
        public DateTime EndTime { get; set; }

        [NotMapped]
        [JsonPropertyName("time")]
        public string Time
        {
            get
            {
                return StartTime.ToString("HH:mm") + "~" + EndTime.ToString("HH:mm");
            }
            set
            {
                string[] splitStr = { "~" };
                string[] words = value.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                if (words.Count() == 2)
                {
                    DateTime startTime, endTime;
                    if (DateTime.TryParseExact(
                        words[0],
                        "HH:mm", null, System.Globalization.DateTimeStyles.None, out startTime))
                        StartTime = startTime;
                    if (DateTime.TryParseExact(
                        words[1],
                        "HH:mm", null, System.Globalization.DateTimeStyles.None, out endTime))
                        EndTime = endTime;
                }
            }
        }


        [JsonIgnore]
        public Guid ReservationGuid { get; set; }

        [JsonIgnore]
        public Reservation Reservation { get; set; } = null!;
    }
}
