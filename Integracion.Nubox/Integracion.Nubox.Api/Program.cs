using DotNetEnv;
using Integracion.Nubox.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


Env.Load();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                     .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddIntegracionNuboxDependencias(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Integracion.Nubox.Api v1");
    c.RoutePrefix = "swagger";
});
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

Console.WriteLine("\n==========================================");
Console.WriteLine("🚀 API INICIADA CORRECTAMENTE");
Console.WriteLine("==========================================");
if (isDocker)
{
    Console.WriteLine("📍 Docker: http://localhost:80");
    Console.WriteLine("🔷 Swagger: http://localhost:80/swagger");
    Console.WriteLine("🔷 Health:  http://localhost:80/health");
}
else
{
    Console.WriteLine("📍 HTTPS: https://localhost:7224");
    Console.WriteLine("📍 HTTP:  http://localhost:5221");
    Console.WriteLine("🔷 Swagger: https://localhost:7224/swagger");
    Console.WriteLine("🔷 Health:  https://localhost:7224/health");
}
Console.WriteLine("==========================================\n");

app.UseHttpsRedirection();
app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.AddIntegracionNuboxEndpoints();
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    service = "Integracion.Nubox.Api"
}))
            .WithName("HealthCheck2")
            .WithTags("Health")
            .AllowAnonymous();

app.AddAuthSeed();
app.AddIntegracionNuboxSeed();

app.Run();
public partial class Program { }