using ProyectoI.Repositories;

var builder = WebApplication.CreateBuilder(args);
//Program.cs esto es lo que se agrega
builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar la inyección de dependencias para ITareasRepository
builder.Services.AddScoped<ITareasRepository>(sp =>
    new TareasRepository(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Connection string not found")));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
