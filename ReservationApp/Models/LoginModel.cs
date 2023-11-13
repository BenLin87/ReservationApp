using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ReservationApp.Models
{
    public class LoginModel
    {
        [JsonProperty("name")]
        [Required(ErrorMessage ="Account name is required.")]
        [MinLength(5, ErrorMessage = "Customer name must be at least {1} characters long.")]
        [MaxLength(10, ErrorMessage = "Customer name cannot exceed {1} characters.")]
        public string name { get; set; } = string.Empty;

        [JsonProperty("password")]
        [Required(ErrorMessage ="Password is required.")]
        [MinLength(5, ErrorMessage = "Customer name must be at least {1} characters long.")]
        [MaxLength(10, ErrorMessage = "Customer name cannot exceed {1} characters.")]
        public string password { get; set; } = string.Empty;
    }
}
