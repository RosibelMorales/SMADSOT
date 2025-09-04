using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Smadot.Utilities.Reporting.Implementacion;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.DataBase;
using Smadot.SolicitudFormaValorada.model.Negocio;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.GestionTokens;
using System.Globalization;
using System.Text;
using Smadot.Utilities.CustomAttributes.Filters.Interfaces;
using Smadot.Utilities.CustomAttributes.Filters.Classes;
using Smadot.Models.GenericProcess;

var builder = WebApplication.CreateBuilder(args); builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true); ;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SmadotDbContext>(options =>
    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Smadot_SolicitudFormaValorada_Api", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
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
builder.Services.AddAuthentication(jtw =>
{
    jtw.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    jtw.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jtw =>
{
    jtw.SaveToken = true;
    jtw.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:ClaveSecreta"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
    builder.WithOrigins()
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    );
});



builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<ISolicitudFormaValoradaNegocio, SolicitudFormaValoradaNegocio>();
builder.Services.AddScoped<IUserResolver, UserResolver>();
builder.Services.AddScoped<IFoliosCanceladosNegocio, FoliosCanceladosNegocio>();
builder.Services.AddScoped<ICircularNegocio, ConsultaCircularNegocio>();
builder.Services.AddScoped<IExentoNegocio, ExentoNegocio>();
builder.Services.AddScoped<IReposicionNegocio, ReposicionNegocio>();
builder.Services.AddScoped<IRefrendoNegocio, RefrendoNegocio>();
builder.Services.AddScoped<IAdministrativaNegocio, AdministrativaNegocio>();
builder.Services.AddScoped<IConsultaTablaMaestraNegocio, ConsultaTablaMaestraNegocio>();
builder.Services.AddScoped<IConstanciaUltimaVerificacionNegocio, ConstanciaUltimaVerificacionNegocio>();
builder.Services.AddScoped<IPdfBuider, PdfBuider>();
builder.Services.AddScoped<IApiKeyValidator, ApiKeyValidator>();
builder.Services.AddScoped<ITestificacionNegocio, TestificacionNegocio>();
builder.Services.AddScoped<IDashboardNegocio, DashboardNegocio>();
builder.Services.AddScoped<SmadsotGenericInserts>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
