using MechAppBackend.AppSettings;
using MechAppBackend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
            builder => builder.WithOrigins("http://localhost:3000", "https://mechapp-frontend.vercel.app", "https://fronttest.motodex.org") // Replace with your client's URL
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader());
});

builder.Services.AddDbContext<MechAppContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MechAppConnection")));
builder.Services.AddDbContext<MechappClientsContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MechAppClientsConnection")));

var app = builder.Build();

GetAppData appData = new GetAppData();

appData.GetAppDataMethod();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
