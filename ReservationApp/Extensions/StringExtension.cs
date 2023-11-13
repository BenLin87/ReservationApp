using System.Text.Json;
using ReservationApp.Models.JsonResponseStructs;

namespace ReservationApp.Extensions
{
    public static class StringExtension
    {
        public static string ToJson(this Object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T? ToObject<T>(this string jsonStr)
        {
            T? obj = JsonSerializer.Deserialize<T>(jsonStr);
            return obj;
        }
    }
}
