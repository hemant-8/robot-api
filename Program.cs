using robot_controller_api.Persistence;
using robot_controller_api.Authentication;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "Backend service for robot simulator",
        Contact = new OpenApiContact
        {
            Name = "Hemant Sachdeva",
            Email = "hemantsachdeva19@email.com"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // 🔥 FIX: same name as authentication scheme
    options.AddSecurityDefinition("BasicAuthentication", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Enter username and password"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicAuthentication"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandEF>();
builder.Services.AddScoped<IMapDataAccess, MapEF>();
builder.Services.AddScoped<RobotContext>();
builder.Services.AddScoped<UserDataAccess>();

// 🔥 Authentication
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication", null);

// 🔥 Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireRole("Admin"));

    options.AddPolicy("UserOnly",
        policy => policy.RequireRole("Admin", "User"));
});

var app = builder.Build();

app.UseStaticFiles();

// 🔥 Swagger (works in Docker + no popup)
app.UseSwagger();
app.UseSwaggerUI(setup =>
{
    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1");
    setup.RoutePrefix = string.Empty;
    setup.InjectStylesheet("/styles/theme-flattop.css");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();