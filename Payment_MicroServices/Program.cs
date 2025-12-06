// Program.cs (top)
using Microsoft.EntityFrameworkCore;
using Payment_MicroServices.Model;
using Payment_MicroServices.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add CORS (allow any origin for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDev",
        policy => policy
            .AllowAnyOrigin()    // Accept any origin (DEV ONLY). Prefer .WithOrigins("https://localhost:5500") in prod
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ... other registrations
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myconn")));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS BEFORE MapControllers()
app.UseCors("AllowDev");

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
