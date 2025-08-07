using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleApi.Data;
using SimpleApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpContextAccessor for JWT claims access
builder.Services.AddHttpContextAccessor();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=testdb.sqlite"));

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

// Add CRM services
builder.Services.AddScoped<SimpleApi.Services.CRM.ILeadService, SimpleApi.Services.CRM.LeadService>();
builder.Services.AddScoped<SimpleApi.Services.CRM.IContactService, SimpleApi.Services.CRM.ContactService>();
builder.Services.AddScoped<SimpleApi.Services.CRM.IDealService, SimpleApi.Services.CRM.DealService>();
builder.Services.AddScoped<SimpleApi.Services.CRM.ICrmTaskService, SimpleApi.Services.CRM.CrmTaskService>();
builder.Services.AddScoped<SimpleApi.Services.CRM.ICrmFormTemplateService, SimpleApi.Services.CRM.CrmFormTemplateService>();
builder.Services.AddScoped<SimpleApi.Services.CRM.ICrmFormSubmissionService, SimpleApi.Services.CRM.CrmFormSubmissionService>();

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
var key = Encoding.ASCII.GetBytes(jwtKey);

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
