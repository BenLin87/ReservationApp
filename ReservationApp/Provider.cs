namespace ReservationApp;

public record Provider(string Name, string Assembly)
{
    public static readonly Provider PostgreSql =
        new(nameof(PostgreSql), typeof(PostgreSql.Marker).Assembly.GetName().Name!);

    public static readonly Provider LocalDb =
        new(nameof(LocalDb), typeof(LocalDb.Marker).Assembly.GetName().Name!);
}

