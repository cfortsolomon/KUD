using KUD.Data;
using KUD.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISuggestionService, SuggestionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5173",
                    "https://app.koinoniaushers.cloud",
                    "http://76.13.42.145:5000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ CORRECT ORDER
app.UseCors("AllowSpecificOrigins");       // 1. CORS first
app.UseHttpsRedirection();                 // 2. Then HTTPS redirect
app.UseRouting();                          // 3. Then routing
app.UseAuthentication();                   // 4. Authentication (you were missing this!)
app.UseAuthorization();                    // 5. Authorization

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();