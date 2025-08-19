using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleApi.Data;
using SimpleApi.Services;
using System;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Only enforce HTTPS in Development
if (builder.Environment.IsDevelopment())
{
	builder.WebHost.ConfigureKestrel(options =>
	{
		options.ListenAnyIP(5000); // HTTP
		options.ListenAnyIP(5001, listenOptions =>
		{
			listenOptions.UseHttps(); // only for local dev cert
		});
	});
}
else
{
	builder.WebHost.ConfigureKestrel(options =>
	{
		options.ListenAnyIP(5000); // only HTTP in production
	});
}

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(o =>
{
	o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpContextAccessor for JWT claims access
builder.Services.AddHttpContextAccessor();

// Add Entity Framework
builder.Configuration
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
	.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' is not configured. " +
        "Please set the ConnectionStrings__DefaultConnection environment variable or " +
        "add DefaultConnection to appsettings.json");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(connectionString));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleOrderService, SaleOrderService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICompanyNatureService, CompanyNatureService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IServiceTypeService, ServiceTypeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

// Add HttpClient for external API calls
builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();

// Add background services
builder.Services.AddHostedService<DeadlineNotificationService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-secret-key-must-be-at-least-256-bits-long-for-security";
Console.WriteLine($"JWT Key length: {builder.Configuration["JWT:Key"]?.Length}");
Console.WriteLine($"JWT Key value: {jwtKey}");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "SimpleApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "SimpleApi",
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Create database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
