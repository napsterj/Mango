using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCart.API;
using Mango.Services.ShoppingCart.API.Data;
using Mango.Services.ShoppingCart.API.Services;
using Mango.Services.ShoppingCart.API.Services.IServices;
using Mango.Services.ShoppingCart.API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter the bearer authorization `Bearer Generated-JWT-Token`",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = JwtBearerDefaults.AuthenticationScheme,
				}
			}, new string[]{}
		}
	});
});

builder.Services.AddDbContext<CartDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiHttpHandler>();
builder.Services.AddHttpClient("Product", config => {
	config.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductApi"]);
}).AddHttpMessageHandler<BackendApiHttpHandler>();

builder.Services.AddHttpClient("Coupon", config =>
{
	config.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponApi"]);
}).AddHttpMessageHandler<BackendApiHttpHandler>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

var jwtSetting = builder.Configuration.GetSection("ApiSettings");

var issuer = jwtSetting.GetValue<string>("Issuer");
var audience = jwtSetting.GetValue<string>("Audience");
var secret = jwtSetting.GetValue<string>("Secret");

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(config =>
{
	config.TokenValidationParameters = new TokenValidationParameters
	{
		ValidAudience = audience,
		ValidIssuer = issuer,
		ValidateAudience = true,
		ValidateIssuer = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),			
	};
});

builder.Services.AddAuthorization();

IMapper mapper = MapperConfig.TranslateDtoEntities().CreateMapper();
builder.Services.AddSingleton(mapper);


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

app.Run();


void ApplyMigration()
{
	using(var scope = app.Services.CreateScope())
	{
	   var db =	scope.ServiceProvider.GetRequiredService<CartDbContext>();

		if (db.Database.GetPendingMigrations().Any())
		{
			db.Database.Migrate();
		}
	}
}