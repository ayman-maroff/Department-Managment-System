using Departments.API.Data;
using Departments.API.Mapping;
using Departments.API.Middlewares;
using Departments.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Hangfire;
using Departments.API.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/DepartmentTask.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DeparmentsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DepartmentsConnectionString")));
builder.Services.AddScoped<IDepartmentRepository, SqlDepartmentRepository>();
builder.Services.AddScoped<ILogoRepository, LocalLogoRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DepartmentsConnectionString"));
});

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Logos")),
    RequestPath = "/Logos"
});

app.UseAuthorization();

app.MapControllers();

app.Run();
