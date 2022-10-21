using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using POS.Models;
using POS.Services;

var builder = WebApplication.CreateBuilder(args);

var polilicyAuth = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(op =>
{
    op.Filters.Add(new AuthorizeFilter(polilicyAuth));
});
builder.Services.AddTransient<IServiceUser, ServiceUser>();
//builder.Services.AddTransient<ITransactionsRepository, TransactionsRepository>();
//builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
//builder.Services.AddTransient<IDashboardService, DashboardService>();
//builder.Services.AddTransient<ISubscriptionsRepository, SubscriptionsRepository>();
//builder.Services.AddTransient<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUserStore<User>, UserStore>();
builder.Services.AddTransient<SignInManager<User>>();
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/Users/Login";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();
