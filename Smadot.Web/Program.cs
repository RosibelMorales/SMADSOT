using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Smadot.Utilities.Reporting.Implementacion;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Implementacion;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Utilities.ServicioMultas;
using Smadot.Web.Interface;
using System.Globalization;
using System.Text;
using Smadot.Models.Entities.GoogleReCaptchaService;
using Smadot.Web.Helper.Operaciones.GoogleCaptcha;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);;
//Custom Error Logger


builder.Services.AddAuthentication(jtw =>
{
    jtw.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    jtw.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jtw =>
{
    jtw.SaveToken = true;
    jtw.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:ClaveSecreta"]))
    };


});

//private const string enUSCulture = "es-MX";
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo( "es-MX"),
    };

    options.DefaultRequestCulture = new RequestCulture(culture: "es-MX", uiCulture: "es-MX");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
    {
        // My custom request culture logic
        return await Task.FromResult(new ProviderCultureResult("es-MX"));
    }));
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
builder.Services.AddScoped<IUserResolver, UserResolver>();
builder.Services.AddScoped<IPdfBuider, PdfBuider>();
builder.Services.AddScoped<IExcelBuilder, ExcelBuilder>();
builder.Services.AddScoped<IConsultaVehiculoServicio, ConsultaVehiculoServicio>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToUInt32(builder.Configuration["TiempoExpiracionSesion"]));
});

#region CanalProxy
builder.Services.AddHttpClient("serviciosAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UrlAPI"]);
})
.ConfigureHttpMessageHandlerBuilder((action) =>
{
    new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});

builder.Services.AddTransient<IProxyWebAPI, ProxyWebAPI>(services => new ProxyWebAPI(services.GetService<IHttpClientFactory>(), "serviciosAPI", services.GetService<IUserResolver>()));
builder.Services.AddTransient(typeof(VerifyTokenReCaptchaHelper));
builder.Services.Configure<GoogleReCaptchaConfig>(builder.Configuration.GetSection("GoogleReCaptchaService"));
#endregion


builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();



var app = builder.Build();
var supportedCultures = new[] { "es-MX" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //    app.UseExceptionHandler("/Error");
    //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    try
    {
        var JWToken = context.Session.GetString("WebSiteSMADSOT");
        if (!string.IsNullOrEmpty(JWToken))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
        }
        await next();
    }
    catch (Exception ex)
    {

    }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    //endpoints.MapHub<Smadot.Web.Hubs.NotificacionHub>("/notificacionHub");
});

app.Run();
