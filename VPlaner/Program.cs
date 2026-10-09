using VPlaner.Components;
using VPlaner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using VPlaner.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext mit SQLite registrieren
var connectionString = builder.Configuration.GetConnectionString("Default") 
                       ?? "Data Source=vplaner.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<AttendanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await DataSeeder.SeedAsync(app.Services);
app.Run();
