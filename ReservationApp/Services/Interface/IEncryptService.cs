namespace ReservationApp.Services.Interface
{
    public interface IEncryptService
    {
        string Key{ get; set; }
        string Iv { get; set; }

        string Encrypt(string data);
        string Encrypt(string data, string key, string iv );

        string Decrypt(string data);
        string Decrypt(string data, string key, string iv);
    }
}
