using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Features.Asistencia.BackgroundServices;
using Integracion.Nubox.Api.Features.Asistencia.Endpoints;
using Integracion.Nubox.Api.Features.Asistencia.HealthChecks;
using Integracion.Nubox.Api.Features.Asistencia.Services;
using Integracion.Nubox.Api.Features.Auth.Endpoints;
using Integracion.Nubox.Api.Features.Nomina.Endpoints;
using Integracion.Nubox.Api.Features.Nomina.Publishers;
using Integracion.Nubox.Api.Features.Nomina.Subscribers;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Repositories;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Seeds;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Seeds;
using Integracion.Nubox.Api.Infrastructure.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Integracion.Nubox.Api.Extensions
{
    public static class IntegracionNuboxExtensions
    {
        public static void AddIntegracionNuboxDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            AddConfiguracionJson(services);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EntryPoint).Assembly));
            AddDbContext(services, configuration);
            AddAuth(services, configuration);
            AddRepositories(services);
            AddIntegracionSettings(services, configuration);
            AddConnectionFactoryRabbitMq(services, configuration);
            AddPublishers(services);
            AddSubscribers(services);
            AddBulkServices(services);
            AddAsistenciaServices(services, configuration);
            AddHealthChecks(services, configuration);
            AddBackgroundServices(services);

            services.AddSingleton(sp =>
            {
                var connectionString = GetRabbitMQConnectionString(configuration);
                var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });
        }
        public static void AddIntegracionNuboxEndpoints(this WebApplication app)
        {
            app.AddAuthEndpoints();
            app.AddNominaEndpoints();
            app.AddAsistenciaEndpoints();
        }

        public static void AddAuthSeed(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthContext>();
            db.Database.Migrate();
            AuthSeed.Seed(db);
        }

        public static void AddIntegracionNuboxSeed(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IntegracionNuboxContext>();
            db.Database.Migrate();
            CompaniaSeed.Seed(db);
            TrabajadorSeed.Seed(db);
            ConfiguracionPartnerSeed.Seed(db);
        }

        private static void AddIntegracionSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IntegracionSettings>(configuration.GetSection("IntegracionSettings"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<IntegracionSettings>>().Value);
            services.AddTransient<IIntegracionSettingsProvider, IntegracionSettingsProvider>();
        }

        private static void AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                var secret = configuration["JwtSettings:Secret"];
                if (string.IsNullOrEmpty(secret))
                    throw new InvalidOperationException("JWT Secret is not configured.");
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = configuration["JwtSettings:ValidAudience"],
                    ValidIssuer = configuration["JwtSettings:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };
            });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Integracion.Nubox.Api",
                    Version = "v1",
                    Description = "API de Integración para Sistema Nubox"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
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
                        Array.Empty<string>()
                    }
                });

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });
        }

        private static void AddConfiguracionJson(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.WriteIndented = true;
                options.SerializerOptions.IncludeFields = true;
            });
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AuthConnection")));

            services.AddDbContext<IntegracionNuboxContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IntegracionNuboxConnection")));
        }
        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICompaniaRepository, CompaniaRepository>();
            services.AddScoped<IBitacoraSincronizacionRepository, BitacoraSincronizacionRepository>();
            services.AddScoped<ITrabajadorRepository, TrabajadorRepository>();
            services.AddScoped<IRegistroAsistenciaRepository, RegistroAsistenciaRepository>();
            services.AddScoped<ITransaccionSincronizacionRepository, TransaccionSincronizacionRepository>();
            services.AddScoped<IConfiguracionPartnerRepository, ConfiguracionPartnerRepository>();
            services.AddScoped<IResumenAsistenciaRepository, ResumenAsistenciaRepository>();
        }

        private static void AddPublishers(this IServiceCollection services)
        {
            services.AddSingleton<ISincronizarNominaPublisher, SincronizarNominaPublisher>();
        }

        private static void AddConnectionFactoryRabbitMq(this IServiceCollection service,
                IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("IntegracionSettings").Get<IntegracionSettings>()
                ?? throw new InvalidOperationException("IntegracionSettings is not configured.");
            var rabbitConfiguration = appSettings.RabbitConfiguration;

            var factory = new ConnectionFactory
            {
                HostName = rabbitConfiguration.HostRabbitMQ,
                UserName = rabbitConfiguration.Username,
                Password = rabbitConfiguration.Password,
                Port = rabbitConfiguration.PortRabbitMQ,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                SocketReadTimeout = TimeSpan.FromSeconds(30),
                SocketWriteTimeout = TimeSpan.FromSeconds(30),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            service.AddSingleton(serviceProvider => factory);
        }

        private static void AddSubscribers(this IServiceCollection services)
        {
            services.AddHostedService<SincronizarNominaSubscriber>();
        }

        private static void AddBulkServices(this IServiceCollection services)
        {
            services.AddScoped<IBulkInsertService, BulkInsertService>();
            services.AddScoped<IEfBulkService, EfBulkService>();
        }

        private static void AddAsistenciaServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAsistenciaProcessorService, AsistenciaProcessorService>();
            services.AddScoped<IResumenAsistenciaService, ResumenAsistenciaService>();
            services.AddScoped<IExcelProcessorService, ExcelProcessorService>();
            services.AddScoped<IAsistenciaValidatorService, AsistenciaValidatorService>();
            services.AddScoped<INotificationService, NotificationService>();
        }
        private static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks() 
                .AddSqlServer(
                    connectionString: configuration.GetConnectionString("IntegracionNuboxConnection")!,
                    name: "database-integracion",
                    tags: ["database", "sql", "integracion"])
                .AddSqlServer(
                    connectionString: configuration.GetConnectionString("AuthConnection")!,
                    name: "database-auth",
                    tags: ["database", "sql", "auth"])
                .AddRabbitMQ(
                    name: "rabbitmq",
                    tags: ["messaging", "rabbitmq"])
                .AddCheck<AsistenciaHealthCheck>(
                    name: "asistencia-processor",
                    tags: ["business", "asistencia"]);
        }
        private static void AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<AsistenciaProcessingService>();
            services.AddHostedService<ResumenGeneratorService>();
            services.AddHostedService<TransactionMonitoringService>();
        }

        private static string GetRabbitMQConnectionString(IConfiguration configuration)
        {
            var rabbitSettings = configuration.GetSection("IntegracionSettings:RabbitConfiguration");
            var host = rabbitSettings["HostRabbitMQ"] ?? "localhost";
            var port = rabbitSettings["PortRabbitMQ"] ?? "5672";
            var username = rabbitSettings["Username"] ?? "guest";
            var password = rabbitSettings["Password"] ?? "guest";

            return $"amqp://{username}:{password}@{host}:{port}/";
        }
    }
}
