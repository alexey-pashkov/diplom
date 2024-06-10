using System.Text;
using AuthService;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5293);
    options.ListenAnyIP(7139, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});

var configBuilder = new ConfigurationBuilder();
configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
var config = configBuilder.Build();

builder.Configuration.AddConfiguration(config);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatDbContext>(options => 
    options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration["ConnectionStrings:DBConnection"])
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
           .EnableSensitiveDataLogging());

var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? throw new Exception();

builder.Services.AddCors(options => 
{
    options.AddDefaultPolicy(policyBuilder =>
    {
       policyBuilder.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(origins)
        .AllowCredentials()
        .WithExposedHeaders("Authorization");; 
    });
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddAuthentication((options) => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:AppKey"]))
    };
}).AddJwtBearer("RefreshTokenScheme", options =>
{
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = true,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:AppKey"]))
    };
});

// builder.Services.AddAuthorization(options =>
//     {
//         options.DefaultPolicy = new AuthorizationPolicyBuilder()
//             .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//             .RequireAuthenticatedUser()
//             .Build();
        
//         options.AddPolicy("RefreshTokenPolicy", policy =>
//         {
//             policy.AddAuthenticationSchemes("RefreshTokenScheme");
//             policy.RequireAuthenticatedUser();
//         });
//     });

builder.Services.AddSingleton(typeof(JwtService));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();

