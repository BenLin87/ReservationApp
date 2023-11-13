using Microsoft.AspNetCore.Authentication.Cookies;
using ReservationApp.Models;
using System.Security.Claims;

namespace ReservationApp.Services.Interface
{
    public interface ILoginService
    {
        bool Logout();

        bool Login(string name, string password);

        bool Login(LoginModel? loginData);

        string? GetCurrentUserName();
    }
}
