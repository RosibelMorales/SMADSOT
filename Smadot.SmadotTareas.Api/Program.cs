using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Smadot.Models.DataBase;
using Smadot.Models.GenericProcess;
using Smadot.SmadotTareas.Negocio.Negocio;
using Smadot.Utilities.CustomAttributes.Filters;
using Smadot.Utilities.CustomAttributes.Filters.Classes;
using Smadot.Utilities.CustomAttributes.Filters.Interfaces;
using Smadot.Utilities.GestionTokens;
using System.Globalization;
using Smadsot.Historico.Models.DataBase;
using System.Text;

var builder = WebApplication.CreateBuilder(args); builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true); ;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionHistoricoString = builder.Configuration.GetConnectionString("SmadsotHistoricoConnection");
builder.Services.AddDbContext<SmadotDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<SmadsotHistoricoDbContext>(options =>
options.UseSqlServer(connectionHistoricoString));
// builder.Services.Configure<TransactionManagerSettings>(options =>
//         {
//             options.DistributedTransactionManager.Enabled = true;
//         });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Smadot_Tareas_Api", Version = "v1" });
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
builder.Services.AddScoped<SmadsotGenericInserts>();
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
builder.Services.AddAuthentication(jwt =>
{
    jwt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    jwt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
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
    options.AddPolicy("CorsPolicy", builder =>
    builder.WithOrigins()
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true));
});
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<ITareasCitasNegocio, TareasCitasNegocio>();
builder.Services.AddScoped<ITareasVerificacionNegocio, TareasVerificacionNegocio>();
builder.Services.AddScoped<ICalibracionesDestiempoNegocio, CalibracionesDestiempoNegocio>();
builder.Services.AddScoped<IUserResolver, UserResolver>();
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
