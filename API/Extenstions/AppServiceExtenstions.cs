using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Service;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Extenstions;

public static class AppServiceExtenstions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();  
        services.AddScoped<IUserRepository, UserRepositiry>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("cloudinarySettings"));
        return services;
    }
}
