using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddControllersWithViews();

// Cookie authentication (cookies + paths)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login"; // simple default
        options.Cookie.Name = "GOTGAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Register IHttpContextAccessor for use in views/controllers
builder.Services.AddHttpContextAccessor();

// In-memory store and password hasher
builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// --- Build app ---
var app = builder.Build();

// Seed data (ensure seeding runs once at startup)
using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetRequiredService<InMemoryStore>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    SeedData.Seed(store, hasher);
}

// --- Middleware pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
