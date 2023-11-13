using System.Text.Json.Serialization;
using ReservationApp.Models.Database;
using ReservationApp.Models.Database.Entities;

namespace ReservationApp.Models.JsonResponseStructs
{
    public class ModifiedOrderJson
    {
        [JsonPropertyName("id")]
        public string? OrderId { get; set; }

        [JsonPropertyName("addReservations")]
        public List<Reservation> AddReservations { get; set; } = new List<Reservation>();

        [JsonPropertyName("deleteReservations")]
        public List<string> DeleteReservations { get; set; } = new List<string>();
    }
}
