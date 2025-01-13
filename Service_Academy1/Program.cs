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

// Add session services
builder.Services.AddDistributedMemoryCache(); // Add this line for caching session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});

// Register the ArliAIService
builder.Services.AddHttpClient<ArliAIService>(); // Register HttpClient
builder.Services.AddSingleton<ArliAIService>(); // Register the custom service
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<LogSystemUsageService>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ProfileImageActionFilter>();

});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
});
var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Admin/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();
