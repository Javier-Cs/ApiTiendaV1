using ApiTiendaV1.Data;
using ApiTiendaV1.Repositorios.Auth;
using ApiTiendaV1.Repositorios.ClienteRop;
using ApiTiendaV1.Repositorios.PagoRop;
using ApiTiendaV1.Repositorios.VentaRop;
using ApiTiendaV1.Servicios.AuthSrv;
using ApiTiendaV1.Servicios.ClienteSrv;
using ApiTiendaV1.Servicios.PagoSrv;
using ApiTiendaV1.Servicios.PeopleSrv;
using ApiTiendaV1.Servicios.Streams;
using ApiTiendaV1.Servicios.VentaSrv;
using ApiTiendaV1.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddHttpClient<StreamService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});


//
builder.Services.AddAuthentication( options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateLifetime = true,
        
        ClockSkew = TimeSpan.Zero
    };

    // Leer el JWT desde la cookie HttpOnly
    options.Events = new JwtBearerEvents {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token)) {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});


// Base de datos
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

// AGREGAR SERVICIO
builder.Services.AddScoped<IClienteRepo, ClienteRepo>();
builder.Services.AddScoped<IVentaRepo, VentaRepo>();
builder.Services.AddScoped<IPagoRepo, PagoRepo>(); 
builder.Services.AddScoped<UserAuthRepo>();
builder.Services.AddScoped<ContarIntentosRepo>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IPagoService, PagoService>();


builder.Services.Configure<ForwardedHeadersOptions>(
    options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    }
);




// rate limit de middleware
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "login",
        config =>
        {
            config.PermitLimit = 21;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        }
    );
});



// CORS

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAstroApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:4321",
                "https://legumfrutsa.com",
                "https://apioper.legumfrutsa.com",
                "https://legumfrutsa.com",
                "http://localhost:4200",
                "https://radiosys.legumfrutsa.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});


var app = builder.Build();

// Configuracion swagger
app.UseSwagger();   

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Operaciones V1");
    c.RoutePrefix = "swagger";
});


// MODO DESARROLLO
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseCors("AllowAstroApp");
//app.UseMiddleware<CsrfMiddleware>();


//user limit
//app.UseRateLimiter();

// configuracion de ip
//app.UseForwardedHeaders();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
