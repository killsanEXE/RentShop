using System.Text;
using System.Text.Json.Serialization;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


// builder.Services.AddControllers()
//     .AddJsonOptions(f => {
//         f.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//     });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

<<<<<<< HEAD
builder.Services.AddCors(options => {
    options.AddPolicy(name: "_myAllowSpecificOrigins", builder => {
        builder
        .AllowAnyHeader()
        .WithOrigins("https://localhost:4200")
        .AllowAnyMethod()
        .AllowCredentials();
=======
if (env == "Docker") {
    builder.Services.AddCors(x => x.AddPolicy("Any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "RentShop.Api", Version = "v1" });
>>>>>>> 9e0290001d61a3c00d2e70407f04f8b621357f8a
    });
} else {
    builder.Services.AddCors(options => {
        options.AddPolicy(name: "_myAllowSpecificOrigins", builder => {
            builder
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });
}

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

builder.Services.AddIdentityCore<AppUser>(opt => 
{
    opt.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddRoleValidator<RoleValidator<AppRole>>()
    .AddEntityFrameworkStores<ApplicationContext>();

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

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try{
        var context = services.GetRequiredService<ApplicationContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        await context.Database.MigrateAsync();
        await Seed.SeedUsers(userManager, roleManager);
        logger.LogError($"{env}-------------------------------finish seed");
    }
    catch(Exception ex)
    {
        
        logger.LogError(ex, "An error occured during migration");
    }
}

app.UseMiddleware<ExceptionMiddleware>();
if (env == "Docker")
{
    app.UseCors("Any");
    app.UseSwagger();
    app.UseSwaggerUI();
} else {
    app.UseCors("_myAllowSpecificOrigins");
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("hubs/message");
app.MapHub<PresenceHub>("hubs/presence");


app.Run();
