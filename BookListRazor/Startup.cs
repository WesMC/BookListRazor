using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListRazor.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace BookListRazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment _env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment()) 
            {
                services.AddDbContext<ApplicationDBContext>(option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
            else 
            {
                // Heroku stores unformatted PostgreSQL conn string as DATABASE_URL
                //string _pgConnString = "postgres://blutrxt:4303a7dc4583390df907715e4ce426362092277b86@ec2-3-122.compute-1.amazonaws.com:5432/d4pekkm";
                string _pgConnString = Environment.GetEnvironmentVariable("DATABASE_URL");
                _pgConnString.Replace("//", "");

                char[] delimiters = {':', '/', '@', '?'};
                string[] connStringArr = _pgConnString.Split(delimiters);

                connStringArr = connStringArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                string _pgBuiltConnStr = 
                "host=" + connStringArr[3] +
                ";port=" + connStringArr[4] + 
                ";database=" + connStringArr[5] +
                ";uid=" + connStringArr[1] + 
                ";pwd=" + connStringArr[2] + ";sslmode=Require;TrustServerCertificate=true";


                services.AddDbContext<ApplicationDBContext>(
                options => options.UseNpgsql(_pgBuiltConnStr)
                );
            }
            
            
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
