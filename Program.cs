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
var app = builder.Build();

builder.Services.AddScoped<CreatMissionByAgent>();// הזרקה של יצירת מסימה
builder.Services.AddScoped<CreatMissionByTarget>();// הזרקה של יצירת מסימה

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
