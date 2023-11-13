using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReservationApp.Models.Entities
{
    public class User
    {
        [Key]
        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.Empty;

        [JsonPropertyName("name")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("orders")]
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
