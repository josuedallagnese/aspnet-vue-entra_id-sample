using EntraId.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, configSectionName: "EntraId");

var securityGroups = new SecurityGroups(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(w =>
        w.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(SecurityGroups.Development, policy => policy.RequireClaim("groups", securityGroups[SecurityGroups.Development]));
    options.AddPolicy(SecurityGroups.ProductOwner, policy => policy.RequireClaim("groups", securityGroups[SecurityGroups.ProductOwner]));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapGet("/development", [Authorize(Policy = SecurityGroups.Development)] () => new
{
    WelcomeMessage = "Welcome development"
});

app.MapGet("/product-owner", [Authorize(Policy = SecurityGroups.ProductOwner)] () => new
{
    WelcomeMessage = "Welcome product owner"
});

app.Run();
