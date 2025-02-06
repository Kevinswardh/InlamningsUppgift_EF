using Business.Services;
using Data.Database;
using Data.DatabaseRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ? Lägg till tjänster till DI-container
builder.Services.AddControllersWithViews();

// ? Registrera BaseRepository som generiskt repository
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// ? Registrera specifika repositories
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IProjectLeaderRepository, ProjectLeaderRepository>();

// ? Registrera Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IProjectLeaderService, ProjectLeaderService>();

// ? Registrera AppDbContext för Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Se till att anslutningssträngen är korrekt

// ? Bygg applikationen
var app = builder.Build();

// ? Konfigurera HTTP-request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ? Ställ in standard route-mönster
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
