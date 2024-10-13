using Mango.Services.EmailAPI;
using Mango.Services.EmailAPI.Extension;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));

builder.Services.AddDbContext<EmailDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
});

var optionsBuilder = new DbContextOptionsBuilder<EmailDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);

builder.Services.AddSingleton<IMessaging, Messaging>();
builder.Services.AddSingleton<IEmailCreationMessage, EmailCreationMessage>();
builder.Services.AddSingleton<IEmailService>(new EmailService(optionsBuilder.Options));

var issuer = builder.Configuration.GetSection("JwtOptions").GetValue<string>("Issuer");
var audience = builder.Configuration.GetSection("JwtOptions").GetValue<string>("Audience");
var Key = builder.Configuration.GetSection("JwtOptions").GetValue<string>("Key");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key)),
        ValidIssuer = issuer,
        ValidAudience = audience
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

// Commenting below code to save Azure free credit. Uncomment when using Service bus functionality.
//app.UseServiceBusConsumer();

app.Run();

void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EmailDbContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}
