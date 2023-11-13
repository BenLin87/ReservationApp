using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ReservationApp.Models.Database;
using ReservationApp.Models.Database.Entities;

namespace ReservationApp.Models.Entities
{
    public class Order
    {
        [Key]
        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.Empty;

        //Unique
        [JsonPropertyName("id")]
        [StringLength(8)]
        public string Id { get; set; } = string.Empty;

        [JsonIgnore]
        public Guid UserGuid { get; set; }

        [NotMapped]
        [JsonPropertyName("userName")]
        public string UserName{ get{return User.Name;}}

        [JsonPropertyName("user")]
        public User User { get; set; } = null!;

        [NotMapped]
        [JsonIgnore]
        public int TimeCount
        {
            get
            {
                return Reservations.Sum(r => r.Times.Count);
            }
        }

        [JsonPropertyName("reservations")]
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
