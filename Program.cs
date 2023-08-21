using System.Text;
using FileUploadService.Models;
using FileUploadService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

var swaggerSettings = configuration.GetSection("SwaggerSettings").Get<SwaggerSettings>();
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Add JWT settings to the container
builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
builder.Services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
//builder.Services.AddSingleton(jwtSettings);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddTransient<IEmailService, EmailService>();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.JwtIssuer,
            ValidAudience = jwtSettings.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.JwtKey))
        };
    });

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo { Title = swaggerSettings.Title, Version = swaggerSettings.Version });

    // Add JWT token authentication details
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer",
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securitySchema, new[] { "Bearer" }
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(swaggerSettings.Endpoint, $"{swaggerSettings.Title} {swaggerSettings.Version}");
        c.RoutePrefix = swaggerSettings.RoutePrefix;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add authentication middleware

app.MapControllers();

// app.Run(async context =>
// {
//     await context.Response.WriteAsync("Hello World!");
// });

app.Run();
