using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services (UPDATED)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register the ArliAIService
builder.Services.AddHttpClient<ArliAIService>(); // Register HttpClient
builder.Services.AddSingleton<ArliAIService>(); // Register the custom service
builder.Services.AddScoped<LogSystemUsageService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Admin/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();
