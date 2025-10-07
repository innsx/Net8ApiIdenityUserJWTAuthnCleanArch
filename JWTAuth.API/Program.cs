using JWTAuth.API.LogHandlers;
using JWTAuth.Application.Interfaces;
using JWTAuth.Application.Services;
using JWTAuth.Domain.Entities;
using JWTAuth.Infrastructure.Context;
using JWTAuth.Infrastructure.Options;
using JWTAuth.Infrastructure.Processors;
using JWTAuth.Infrastructure.Respositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDbConnString"));
});

builder.Services.AddDbContext<MovieDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieDbConnString"));
});


//REGISTER & INJECT AuthTokenProcessor
builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();

//REGISTER & INJECT AccountService
builder.Services.AddScoped<IAccountService, AccountService>();

//REGISTER & INJECT UserRepository
builder.Services.AddScoped<IUserRepositories, UserRepository>();

builder.Services.AddScoped<IMovieRepository, MovieRepository>();


//Register a configuration instance of JwtOptions
//Get appsettings.json JwtOptions section and
//BIND JwtOptions section Keys
//to their corresponding JwtOptions class properties
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JwtOptionsKey));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<ApplicationDbContext>();


//REGISTER the Authentication as a SERVICES &
//SPECIFIED JwtBearerDefaults.AuthenticationScheme for all DEFAULT SCHEMEs
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>  //REGISTER JwtBearer as a SERVICE
{
    //GET JwtOptions section & BINDs to JwtOptions object & store it as a local variable 
    var jwtOptions = builder.Configuration
            .GetSection(JwtOptions.JwtOptionsKey).Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

    //VALIDATING the JWT Token based of these OPTION parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret!))
    };

    //we have append JWT in Browser's HTTPOnly/local storage 
    //but NOT appened in the HTTP HEADER
    //so we specified where to find JWT token with the a specific ["CookieName"]
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["ACCESS_TOKEN"];

            return Task.CompletedTask;
        }
    };
});

//Register Authorization as a SERVICE
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Register GlobalExceptionHandler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//register HTTContextAccessor
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    //Installed Scalar.AspNetCore & mapped it as UI testing 
    app.MapScalarApiReference();
}

//_ => { } part is a lambda expression that represents an empty delegate.
//In this specific context, it means that no custom logic is being provided directly within the UseExceptionHandler call
//to handle the exceptions.
app.UseExceptionHandler(_ => { });
////app.UseExceptionHandler("/Error");  
//logic provided directly within the UseExceptionHandler call
//this Specify an error handling path: This redirects the request to a specific path (e.g., /Error)
//where a dedicated controller action or Razor Page handles the error and generates an appropriate response.

app.UseHttpsRedirection();

//use Authentication & Authorization in the MIDDLEWARE
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
