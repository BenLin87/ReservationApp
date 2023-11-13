using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using ReservationApp.Models;
using ReservationApp.Services;
using ReservationApp.Services.Interface;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization(
    LanguageViewLocationExpanderFormat.SubFolder);
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICreateService, CreateService>();
builder.Services.AddScoped<IReadService, ReadService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();
builder.Services.AddScoped<IDeleteService, DeleteService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IEncryptService, EncryptService>();
bool postgreSql = false;

builder.Services.AddDbContext<ReservationContext>(options =>
{
    var config = builder.Configuration;
    var provider = config.GetValue("provider", Provider.LocalDb.Name);
    if (provider == Provider.PostgreSql.Name)
    {
        postgreSql = true;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        options.UseNpgsql(config.GetConnectionString(Provider.PostgreSql.Name),
            x => x.MigrationsAssembly(Provider.PostgreSql.Assembly));
    }
    else
    {
        var connStr = config.GetConnectionString(Provider.LocalDb.Name);

        //If you want to update the localDb in this Solution, set useRootDb = true
        var useRootDb = false;
        if(useRootDb)
        {
            connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""" +
            Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Migrations\ReservationApp.LocalDb\LocalDb\Database.mdf"";Integrated Security=True";
        }

        options.UseSqlServer(connStr,
            x => x.MigrationsAssembly(Provider.LocalDb.Assembly));
    }
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = new PathString("/Login/AdminLoginPage");
    options.ExpireTimeSpan = TimeSpan.FromSeconds(600);
});



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<ReservationContext>();
    if (context != null)
    {
        await ReservationContext.InitializeAsync(context, postgreSql);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/*
#if DEBUG
_ = Task.Run(action: () =>
{
    using (ReservationContext context = new())
    {
        _ = context.Model;
        context.ClearAllData();
    }
});
#endif
*/

var supportedCultures = new List<CultureInfo>()
{
    new CultureInfo("en-US"),
    new CultureInfo("zh-TW"),
    new CultureInfo("zh-Hant")
};

app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    ApplyCurrentCultureToResponseHeaders = true
}) ;

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();


app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = context=>
    {
        context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
        context.Context.Response.Headers.Add("Expires", "-1");
    }
});



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
