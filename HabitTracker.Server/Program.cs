using HabitTracker.Server.Data;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add AuthService, HabitService
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHabitService, HabitService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.MapStaticAssets();

string? GetUserId(HttpContext context) =>
    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

app.MapPost("/api/auth/register", async (RegisterDto dto, IAuthService authService) =>
{
    AuthResponseDto? result = await authService.RegisterAsync(dto);

    if (result == null)
    {
        return Results.BadRequest("Registration Failed");
    }

    return Results.Ok(result);
});

app.MapPost("/api/auth/login", async (LoginDto dto, IAuthService authService) =>
{
    AuthResponseDto? result = await authService.LoginAsync(dto);

    if (result == null)
    {
        return Results.BadRequest("Login Failed");
    }

    return Results.Ok(result);
});

RouteGroupBuilder habitsGroup = app.MapGroup("/api/habits")
    .RequireAuthorization();

// POST - /api/habits - Create a habit
habitsGroup.MapPost("", async (CreateHabitDto dto, IHabitService habitService, HttpContext httpContext) =>
{
    var userId = GetUserId(httpContext);
    if (userId == null) return Results.Unauthorized();

    HabitResponseDto habit = await habitService.CreateHabitAsync(userId, dto);

    return Results.Created($"/api/habits/{habit.Id}", habit);
});

//GET - /api/habits - Get all habits
habitsGroup.MapGet("", async (IHabitService habitService, HttpContext httpContext) =>
{
    var userId = GetUserId(httpContext);
    if (userId == null) return Results.Unauthorized();

    List<HabitResponseDto> habits = await habitService.GetAllHabitsAsync(userId);
    return Results.Ok(habits);
});

//GET - /api/habits/{id} - Get a habit by id
habitsGroup.MapGet("{id}", async (Guid id, IHabitService habitService, HttpContext httpContext) =>
{
    var userId = GetUserId(httpContext);
    if (userId == null) return Results.Unauthorized();

    HabitResponseDto? habit = await habitService.GetHabitAsync(userId, id);
    return habit == null ? Results.NotFound() : Results.Ok(habit);
});

//DELETE - /api/habits/{id} - Delete a habit
habitsGroup.MapDelete("{id}", async (Guid id, IHabitService habitService, HttpContext httpContext) =>
{
    var userId = GetUserId(httpContext);
    if (userId == null) return Results.Unauthorized();

    bool result = await habitService.DeleteHabitAsync(userId, id);

    return result ? Results.NoContent() : Results.NotFound();
});

//PUT - /api/habits/{id} - Update a habit
habitsGroup.MapPut("{id}", async (Guid id, CreateHabitDto dto, IHabitService habitService, HttpContext httpContext) =>
{
    var userId = GetUserId(httpContext);
    if (userId == null) return Results.Unauthorized();

    HabitResponseDto? habit = await habitService.UpdateHabitAsync(userId, dto, id);

    return habit == null ? Results.NotFound() : Results.Ok(habit);
});

app.MapFallbackToFile("/index.html");

app.Run();