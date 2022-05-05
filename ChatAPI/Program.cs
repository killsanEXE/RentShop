using System.Text;
using API.Entities;
using ChatAPI.Data;
using ChatAPI.Helpers;
using ChatAPI.Interfaces;
using ChatAPI.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (env == "Docker") 
{
    builder.Services.AddCors(x => x.AddPolicy("_myDockerSpecieficOrigins", builder => {
        builder
            .AllowAnyHeader()
            .WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowCredentials();
    }));
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "RentShop.Api", Version = "v1" });
    });
} 
else 
{
    builder.Services.AddCors(options => {
        options.AddPolicy(name: "_myAllowSpecificOrigins", builder => {
            builder
            .AllowAnyHeader()
            .WithOrigins("https://localhost:4200")
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });
}

builder.Services.AddIdentityCore<AppUser>(opt => 
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.SignIn.RequireConfirmedEmail = true;
})
    .AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddRoleValidator<RoleValidator<AppRole>>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,    
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("TokenKey"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => 
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddDbContext<ApplicationContext>(options => {    
    string connStr;
    if (env == "Development" || env == "Docker")
    {
        connStr = builder.Configuration.GetConnectionString("Default Connection");
    }
    else
    {
        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        connUrl = connUrl!.Replace("postgres://", string.Empty);
        var pgUserPass = connUrl.Split("@")[0];
        var pgHostPortDb = connUrl.Split("@")[1];
        var pgHostPort = pgHostPortDb.Split("/")[0];
        var pgDb = pgHostPortDb.Split("/")[1];
        var pgUser = pgUserPass.Split(":")[0];
        var pgPass = pgUserPass.Split(":")[1];
        var pgHost = pgHostPort.Split(":")[0];
        var pgPort = pgHostPort.Split(":")[1];

        connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
    }
    options.UseNpgsql(connStr);
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


builder.Services.AddSingleton<PresenceTracker>();
builder.Services.AddAutoMapper(typeof(AutomapperProfiles).Assembly);
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

if (env == "Docker") 
{
    builder.Services.AddSignalR(e => {
        e.MaximumReceiveMessageSize = 102400000;
        e.EnableDetailedErrors = true;
    });
} 
else 
{
    builder.Services.AddSignalR();
}

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationContext>();
    var users = await context.Users.ToListAsync();
    foreach(var i in users)
    {
        System.Console.WriteLine(i.Email);
    }
}


if (env == "Docker")
{
    app.UseCors("_myDockerSpecieficOrigins");
    app.UseSwagger();
    app.UseSwaggerUI();
} 
else 
{
    app.UseCors("_myAllowSpecificOrigins");
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("hubs/message");
app.MapHub<PresenceHub>("hubs/presence");

app.Run();
