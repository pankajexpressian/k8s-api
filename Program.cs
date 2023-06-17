using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using k8s_api.Data;
using System.Xml.Linq;

namespace k8s_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string server, port, dbname, dbuser, dbpassword, con = string.Empty;

            try
            {
                server = Environment.GetEnvironmentVariable("SERVER") ?? string.Empty;
                port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? string.Empty;
                dbname = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? string.Empty;
                dbuser = Environment.GetEnvironmentVariable("SA_USER") ?? string.Empty;
                dbpassword = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? string.Empty;
                con = $"Server={server},{port};Database={dbname};User Id={dbuser}; password={dbpassword};";
                Console.WriteLine($"*********{con}***********");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't read env varibales");
            }

            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(con.ToString() ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}