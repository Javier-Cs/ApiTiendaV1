using ApiTiendaV1.Data;
using ApiTiendaV1.Repositorios.ClienteRop;
using ApiTiendaV1.Repositorios.PagoRop;
using ApiTiendaV1.Repositorios.VentaRop;
using ApiTiendaV1.Servicios.ClienteSrv;
using ApiTiendaV1.Servicios.PagoSrv;
using ApiTiendaV1.Servicios.VentaSrv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();


builder.Services.AddScoped<IClienteRepo, ClienteRepo>();
builder.Services.AddScoped<IVentaRepo, VentaRepo>();
builder.Services.AddScoped<IPagoRepo, PagoRepo>(); 
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IPagoService, PagoService>();


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

app.UseSwagger();
//app.UseSwaggerUI();
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
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseCors("AllowAstroApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
