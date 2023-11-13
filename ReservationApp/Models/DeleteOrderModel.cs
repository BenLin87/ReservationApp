using System.ComponentModel.DataAnnotations;

namespace ReservationApp.Models
{
    public class DeleteOrderModel
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;
    }
}
