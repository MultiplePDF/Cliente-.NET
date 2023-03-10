var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();
// app.UseEndpoints(endpoints => {

//     endpoints.MapControllerRoute(
//         name: "MainPageHome",
//         pattern: "/MainPage/Home",
//         defaults: new { controller = "MainPage", action = "Home" }
//     );

// });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
