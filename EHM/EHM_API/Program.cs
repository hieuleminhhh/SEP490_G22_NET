using EHM_API.Models;
using EHM_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjectSchedule.Authenticate;
using ProjectSchedule.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using EHM_API.Repositories;

namespace EHM_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Schedule API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add CORS policy
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CORSPolicy", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true);
                });
            });

            // Register DbContext
            var configuration = builder.Configuration;
            builder.Services.AddDbContext<EHMDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register JwtTokenGenerator service with appropriate scope
            builder.Services.AddScoped<JwtTokenGenerator>();

            // Register JwtSetting from configuration
            var jwtSettings = new JwtSetting();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            builder.Services.AddSingleton(jwtSettings);

            builder.Services.AddSingleton<IVnPayService, VnPayService>();
            // Add Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };
                });

            builder.Services.AddAutoMapper(typeof(Program)); // Register AutoMapper

            // Register necessary services
            builder.Services.AddScoped<IDishService, DishService>();
            builder.Services.AddScoped<IDishRepository, DishRepository>();

            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
          
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

             builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();

			builder.Services.AddScoped<IComboService, ComboService>();
			builder.Services.AddScoped<IComboRepository, ComboRepository>();

            builder.Services.AddScoped<IGuestRepository, GuestRepository>();
            builder.Services.AddScoped<IGuestService, GuestService>();

            builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();

            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();

			builder.Services.AddScoped<ITableService, TableService>();
			builder.Services.AddScoped<ITableRepository, TableRepository>();

			builder.Services.AddScoped<IReservationService, ReservationService>();
			builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

            builder.Services.AddScoped<ITableReservationRepository, TableReservationRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

			builder.Services.AddScoped<IAccountRepository, AccountRepository>();
			builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();


			builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
			builder.Services.AddScoped<IInvoiceService, InvoiceService>();

            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("CORSPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
			app.UseSession();
			app.MapControllers();

            app.Run();
        }
    }
}
