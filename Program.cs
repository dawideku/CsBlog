using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Dodaj us³ugê DbContext (po³¹czenie z baz¹ danych)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dodaj Identity z domyœlnymi opcjami
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); // np. do resetowania has³a

// Dodaj us³ugê MVC
builder.Services.AddControllersWithViews();

// Dodaj uwierzytelnianie i autoryzacjê
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Konfiguracja aplikacji HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Te dwie linijki s¹ wa¿ne:
app.UseAuthentication(); // obs³uga logowania
app.UseAuthorization();  // sprawdzanie uprawnieñ

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}");

app.Run();
