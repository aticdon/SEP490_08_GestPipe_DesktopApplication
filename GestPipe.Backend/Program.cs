using GestPipe.Backend.Mappings;
using GestPipe.Backend.Models;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.Implementation;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;

namespace GestPipe.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            builder.Services.Configure<GestPipe.Backend.Config.I18nConfig>(builder.Configuration.GetSection("I18n"));

            builder.Services.Configure<MongoDbSettings>(
                builder.Configuration.GetSection("MongoDB"));

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            builder.Services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(builder.Configuration["MongoDB:databaseName"]);
            });
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                            });
            });

            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure settings
            builder.Services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
            builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            builder.Services.Configure<GoogleSettings>(configuration.GetSection("GoogleSettings"));
            builder.Services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

            // Register services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDefaultGestureService, DefaultGestureService>();
            builder.Services.AddScoped<IGestureTypeService, GestureTypeService>();
            builder.Services.AddScoped<IUserGestureConfigService, UserGestureConfigService>();
            builder.Services.AddScoped<ITrainingGestureService, TrainingGestureService>();
            builder.Services.AddScoped<IGestureInitializationService, GestureInitializationService>();

            builder.Services.AddScoped<IUserGestureRequestService, UserGestureRequestService>();
            // Add Services
            //builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<GoogleDriveService>();
            builder.Services.AddScoped<ITopicService,TopicService>();
            builder.Services.AddScoped<ISessionService,SessionService>();
            builder.Services.AddScoped<ICategoryService,CategoryService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings?.Secret ?? "DefaultSecretKey");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings?.Issuer,  // "GestPipe.Backend"
                    ValidAudience = jwtSettings?.Audience,  // "GestPipe"
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(60)  // ✅ THÊM
                };
            });

            // ✅ THÊM AUTHORIZATION SERVICE
            builder.Services.AddAuthorization();

            // ✅ CẬP NHẬT CORS ĐỂ CHO PHÉP AUTHORIZATION HEADER
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()  // ✅ Cho phép tất cả headers
                           .WithExposedHeaders("Authorization");  // ✅ Expose Authorization header
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();

            //// ✅ THÊM (Optional): Custom static file serving cho avatars
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                Path.Combine(app.Environment.WebRootPath, "avatars")),
                RequestPath = "/avatars"
            });
            app.UseHttpsRedirection();

            // ✅ CORS PHẢI ĐẶT SỚM TRƯỚC Authentication
            app.UseCors("AllowAll");

            // ✅ THỨ TỰ MIDDLEWARE QUAN TRỌNG!
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}