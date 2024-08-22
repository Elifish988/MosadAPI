using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Meneger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MosadDBContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<CreatMissionByAgent>();// הזרקה של יצירת משימה
builder.Services.AddScoped<CreatMissionByTarget>();// הזרקה של יצירת משימה
builder.Services.AddScoped<MissionsMeneger>();// הזרקה של יצירת MissionsMeneger
var app = builder.Build();



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
