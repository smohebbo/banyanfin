using Banyan.IOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddWebOptimizer(Banyan.WebOptimizerConfig.Configure);
builder.Services.RegisterMCCCULServiceInstance(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();
app.UseWebOptimizer();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
