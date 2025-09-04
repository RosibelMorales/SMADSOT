using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);;

if (builder.Environment.EnvironmentName.Contains("Production"))
{
    // Add services to the container.
    builder.Configuration
         //.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
         //.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
      .AddJsonFile($"ocelot.prod.json", true, true);
}
else if (builder.Environment.EnvironmentName.Contains("prodsec"))
{
// Add services to the container.
builder.Configuration
  //.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
  //.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
  .AddJsonFile($"ocelot.prodsec.json", true, true);
}
else
{
    // Add services to the container.
    builder.Configuration
         .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
         .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
    //  .AddJsonFile($"ocelot.prod.json", true, true);
}


builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("es-MX"),
                    //new CultureInfo("en-US"),
                };
    options.DefaultRequestCulture = new RequestCulture("es-MX");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            {
                var jwt = new JwtSecurityToken(token);
                return jwt;
            },
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireSignedTokens = false
        };
    });

var app = builder.Build();


app.UseCors("CORSPolicy");
app.UseHttpsRedirection();
app.UseOcelot();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }