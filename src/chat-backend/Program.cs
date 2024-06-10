using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ChatApp;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5044);
    options.ListenAnyIP(7256, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var configBuilder = new ConfigurationBuilder();
configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
var config = configBuilder.Build();

builder.Configuration.AddConfiguration(config);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddLogging();
// builder.Services.AddHttpClient("AuthServicClient", client => 
// {
//     client.BaseAddress = new Uri(builder.Configuration["AuthenticationServer:BaseUrl"]!);
// });

builder.Services.AddDbContext<ChatDbContext>((options) => {
    options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration["ConnectionStrings:DBConnection"]);
});

var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? throw new Exception();

builder.Services.AddCors(options => 
{
    options.AddDefaultPolicy(policyBuilder =>
    {
       policyBuilder.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(origins)
        .AllowCredentials()
        .WithExposedHeaders("Authorization");
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<DbExceptionFilterAttribute>();
}).AddNewtonsoftJson(options => {
    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSignalR();

builder.Services.AddAuthentication((options) => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:AppKey"]!))
    };
}).AddJwtBearer("ChatsTokenScheme", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:ChatsKey"]!))
    };

    options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claims = context.Principal.Claims.ToList();
                    var jwtToken = (JwtSecurityToken)context.SecurityToken;

                    // Пример добавления пользовательского клейма
                    if (jwtToken.Payload.TryGetValue("chatId", out var chatId))
                    {
                        claims.Add(new Claim("chatId", chatId.ToString()));
                    }

                    var identity = new ClaimsIdentity(claims, "JwtBearer");
                    context.Principal.AddIdentity(identity);
                    return Task.CompletedTask;
                }
            };
}).AddJwtBearer("SignalRScheme", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:AppKey"]!))
    };

    options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
});

// builder.Services.AddAuthorization(options =>
//     {
//         options.DefaultPolicy = new AuthorizationPolicyBuilder()
//             .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//             .RequireAuthenticatedUser()
//             .Build();
        
//         options.AddPolicy("ChatsTokenPolicy", policy =>
//         {
//             policy.AddAuthenticationSchemes("ChatsTokenScheme");
//             policy.RequireAuthenticatedUser();
//         });

//         options.AddPolicy("SignalRPolicy", policy => 
//         {
//             policy.AddAuthenticationSchemes("SignalRScheme");
//             policy.RequireAuthenticatedUser();
//         });
//     });

builder.Services.AddSingleton(typeof(JwtService));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

// app.UseHttpsRedirection();

app.Run();



