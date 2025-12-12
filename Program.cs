using InventarioMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace InventarioMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ------------------------------------------
            // 1. Agregar MVC
            // ------------------------------------------
            builder.Services.AddControllersWithViews();

            // ------------------------------------------
            // 2. Registrar DbContext con SQL Server
            // ------------------------------------------
            builder.Services.AddDbContext<InventarioContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            var app = builder.Build();

            // ------------------------------------------
            // 3. Configurar pipeline HTTP
            // ------------------------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            // ------------------------------------------
            // 4. Rutas de MVC
            // ------------------------------------------
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Productos}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
