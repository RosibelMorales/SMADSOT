using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Smadot.Models.DataBase;
using Smadot.Catalogo.Model.Negocio;
using System.Text;
using Smadot.Utilities.GestionTokens;

var builder = WebApplication.CreateBuilder(args);builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);;

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
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Smadot_Cat�logo_Api", Version = "v1" });
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
builder.Services.AddScoped<IAlmacenNegocio, AlmacenNegocio>();
builder.Services.AddScoped<IUserResolver, UserResolver>();
builder.Services.AddScoped<ITipoCertificado, TipoCertificadoNegocio>();
builder.Services.AddScoped<IMotivoTramiteNegocio, MotivoTramiteNegocio>();
builder.Services.AddScoped<ITipoPuestoNegocio, TipoPuestoNegocio>();
builder.Services.AddScoped<ITipoCalibracionNegocio, TipoCalibracionNegocio>();
builder.Services.AddScoped<ITipoAcreditacionNegocio, TipoAcreditacionNegocio>();
builder.Services.AddScoped<INormaAcreditacionNegocio, NormaAcreditacionNegocio>();
builder.Services.AddScoped<IMotivoReporteCredencialNegocio, MotivoReporteCredencialNegocio>();
builder.Services.AddScoped<IMarcaVehiculoNegocio, MarcaVehiculoNegocio>();
builder.Services.AddScoped<ISubMarcaVehiculoNegocio, SubMarcaVehiculoNegocio>();
builder.Services.AddScoped<IMotivoVerificacionNegocio, MotivoVerificacionNegocio>();
builder.Services.AddScoped<ICatalogosVerificacion, CatalogosVerificacion>();
builder.Services.AddScoped<ITipoEquipoNegocio, TipoEquipoNegocio>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
