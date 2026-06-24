using ApiTiendaV1.Data;
using ApiTiendaV1.Repositorios.ClienteRop;
using ApiTiendaV1.Repositorios.PagoRop;
using ApiTiendaV1.Repositorios.VentaRop;
using ApiTiendaV1.Servicios.ClienteSrv;
using ApiTiendaV1.Servicios.PagoSrv;
using ApiTiendaV1.Servicios.PeopleSrv;
using ApiTiendaV1.Servicios.VentaSrv;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]);

// Base de datos
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

// AGREGAR SERVICIO
builder.Services.AddScoped<IClienteRepo, ClienteRepo>();
builder.Services.AddScoped<IVentaRepo, VentaRepo>();
builder.Services.AddScoped<IPagoRepo, PagoRepo>(); 
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IPagoService, PagoService>();




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
                "https://www.legumfrutsa.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
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

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//}

//app.UseHttpsRedirection();
//app.UseDeveloperExceptionPage();


// MODO DESARROLLO

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseCors("AllowAstroApp");
app.UseMiddleware<CsrfMiddleware>();


//user limit
app.UseRateLimiter();

// configuracion de ip
app.UseForwardedHeaders();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
