using HabitTracker.Server.Data;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

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
        Description = "Enter your Auth0 JWT token WITHOUT the 'Bearer ' prefix"
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            NameClaimType = "sub"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IHabitService, HabitService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.MapStaticAssets();

string? GetUserId(HttpContext context) =>
    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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