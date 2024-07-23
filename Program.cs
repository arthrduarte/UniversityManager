using Microsoft.EntityFrameworkCore;
using lab4.DataAccess;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
string dbConnStr = builder.Configuration.GetConnectionString("StudentRecord");
if (dbConnStr != null)
{

    builder.Services.AddDbContext<StudentrecordContext>(
    options => options.UseMySQL(dbConnStr));
}
else
{
    throw new Exception("no connection string obtained");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
