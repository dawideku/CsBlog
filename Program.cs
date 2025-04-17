using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Dodaj us�ug� DbContext (po��czenie z baz� danych)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dodaj Identity z domy�lnymi opcjami
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); // np. do resetowania has�a

// Dodaj us�ug� MVC
builder.Services.AddControllersWithViews();

// Dodaj uwierzytelnianie i autoryzacj�
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

// Te dwie linijki s� wa�ne:
app.UseAuthentication(); // obs�uga logowania
app.UseAuthorization();  // sprawdzanie uprawnie�

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}");

app.Run();
