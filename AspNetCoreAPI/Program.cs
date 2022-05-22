using System;
using System.IO;
using System.Reflection;
using ASPNetCoreAPI.Authorization;
using ASPNetCoreAPI.Core;
using ASPNetCoreAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    var services = builder.Services;
    builder.Services.AddDbContext<DataContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
    services.AddCors();
    services.AddControllers();
    //services.AddDatabaseDeveloperPageExceptionFilter();
    services.AddAutoMapper(typeof(Program));
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ASP .Net Core API",
            Description = "Simple API example",
            TermsOfService = new Uri("https://localhost/terms"),
            Contact = new OpenApiContact
            {
                Name = "liemlylac",
                Url = new Uri("https://github.com/liemlylac")
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://github.com/liemlylac/asp-net-core-api#LICENSE.txt")
            }
        });
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

using (IServiceScope scope = app.Services.CreateScope())
{
    DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();
    dataContext.Database.Migrate();
}

{
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.UseMiddleware<JwtMiddleware>();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}


app.Run();