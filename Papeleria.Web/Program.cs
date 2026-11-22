using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Data;
using Papeleria.Core.Services;
using Papeleria.Web.Data;

var builder = WebApplication.CreateBuilder(args);

var negocioDbPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "papeleria_ventas.db"));
var identityDbPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "papeleria_usuarios.db"));

builder.Services.AddDbContext<PapeleriaDbContext>(options =>
    options.UseSqlite($"Data Source={negocioDbPath}"));

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlite($"Data Source={identityDbPath}"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<IdentityContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddScoped<InventarioVentasService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var coreDb = scope.ServiceProvider.GetRequiredService<PapeleriaDbContext>();
    coreDb.Database.EnsureCreated();

    var idDb = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    idDb.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Administrador", "Vendedor" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    if (await userManager.FindByNameAsync("admin@papeleria.com") == null)
    {
        var admin = new IdentityUser { UserName = "admin@papeleria.com", Email = "admin@papeleria.com", EmailConfirmed = true };
        await userManager.CreateAsync(admin, "1234");
        await userManager.AddToRoleAsync(admin, "Administrador");
    }

    if (await userManager.FindByNameAsync("vendedor@papeleria.com") == null)
    {
        var vend = new IdentityUser { UserName = "vendedor@papeleria.com", Email = "vendedor@papeleria.com", EmailConfirmed = true };
        await userManager.CreateAsync(vend, "1234");
        await userManager.AddToRoleAsync(vend, "Vendedor");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
