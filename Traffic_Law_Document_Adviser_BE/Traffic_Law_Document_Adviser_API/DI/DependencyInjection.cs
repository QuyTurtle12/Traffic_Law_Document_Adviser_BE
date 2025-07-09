using System.Reflection;
using System.Text;
using BusinessLogic.IServices;
using BusinessLogic.MappingProfiles;
using BusinessLogic.Services;
using DataAccess.Constant;
using DataAccess.DTOs.AuthDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.IServices;
using DataAccess.Repositories;
using DataAccess.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Traffic_Law_Document_Adviser_API.DI
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigSwagger(configuration);
            services.AddAuthenJwt(configuration);
            services.AddAuthor(configuration);
            services.AddDatabase(configuration);
            services.ConfigRoute();
            //services.AddInitialiseDatabase();
            services.ConfigCors();
            services.AddRepository();
            services.AddAutoMapper();
            services.AddServices();
            //services.AddOtherServices();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
        }

        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddAuthenJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind JwtSettings from appsettings.json
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Add Authentication + JWT Bearer
            var jwtSection = configuration.GetSection("JwtSettings");
            var jwtConfig = jwtSection.Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
        }

        public static void AddAuthor(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure role‐based policies
            services.AddAuthorization(options =>
            {
                // Only Admin can do certain things
                options.AddPolicy("RequireAdminRole", policy =>
                  policy.RequireRole(RoleConstants.Admin));

                // Expert OR Admin
                options.AddPolicy("RequireExpertOrAdmin", policy =>
                  policy.RequireRole(RoleConstants.Expert, RoleConstants.Admin));

                // Any authenticated User (including Expert/Admin)
                options.AddPolicy("RequireAnyUserRole", policy =>
                  policy.RequireRole(RoleConstants.User, RoleConstants.Expert, RoleConstants.Admin));
            });
        }

        public static void ConfigSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Swagger services with XML comments
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Swagger API",
                    Version = "v1"
                });

                // Add XML Comments
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            // Make all route lowercase
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TrafficLawDocumentDbContext>(options =>
            {
                //options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("MyCnn"));
                options.UseSqlServer(configuration.GetConnectionString("MyCnn"));
            });
        }

        public static void AddRepository(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IUOW, UOW>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        }

        private static void AddAutoMapper(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(UserProfile).Assembly);
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddLogging();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatHistoryService, ChatHistoryService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<ILawDocumentService, LawDocumentService>();
            services.AddScoped<IDocumentCategoryService, DocumentCategoryService>();
            services.AddScoped<IDocumentTagService, DocumentTagService>();

            services.AddScoped<IPdfService, PdfService>();
        }

        //private static void AddOtherServices(this IServiceCollection services)
        //{
        //    services.AddHttpContextAccessor();
        //}
    }
}
