using ESchedule.Services;
using ManufactureHub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddDbContextPool<ManufactureHubContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
    //options.EnableSensitiveDataLogging(true);
}
    /*o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))*/);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ManufactureHubContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = IdentityConstants.ApplicationScheme;
    opt.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseStatusCodePages(async statusCodeContext =>
{
    // using static System.Net.Mime.MediaTypeNames;
    statusCodeContext.HttpContext.Response.ContentType = Text.Plain;

    await statusCodeContext.HttpContext.Response.WriteAsync(
        $"Error. Status code: {statusCodeContext.HttpContext.Response.StatusCode}");
});

app.MapIdentityApi<ApplicationUser>();

//app.Use(async (ctx, next) =>
//{
//    await next();

//    if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
//    {
//        //Re-execute the request so the user gets the error page
//        string originalPath = ctx.Request.Path.Value;
//        ctx.Items["originalPath"] = originalPath;
//        ctx.Request.Path = "/Home/404";
//        await next();
//    }
//});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Task}/{action=Index}/{id?}") //{controller=Home}/{action=Index}/{id?}
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
