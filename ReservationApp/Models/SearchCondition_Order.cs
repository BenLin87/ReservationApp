using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ReservationApp.Models
{
    public class SearchCondition_Order
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonPropertyName("dateFrom")]
        public string DateFrom { get; set; } = string.Empty;

        [JsonPropertyName("dateTo")]
        public string DateTo { get; set; } = string.Empty;

        [JsonPropertyName("displayMode")]
        public DisplayModes DisplayMode { get; set; }

        public enum DisplayModes
        {
            Avaliable = 0,
            Overdue = 1,
            All = 2
        }

        public bool IsEmpty()
        {
            if (UserName == string.Empty &&
                OrderId == string.Empty &&
                DateFrom == string.Empty &&
                DateTo == string.Empty &&
                DisplayMode == DisplayModes.All)
            {
                return true;
            }
            return false;
        }
    }
}
