using System.Text;
using Dmb.Crm.Api.Filters;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Mapper.CrmMapperProfiles;
using Dmb.Crm.Data.Repository.Implementation.Auth;
using Dmb.Crm.Data.Repository.Implementation.Company;
using Dmb.Crm.Data.Repository.Implementation.Contact;
using Dmb.Crm.Data.Repository.Implementation.Location;
using Dmb.Crm.Data.Repository.Implementation.Opportunity;
using Dmb.Crm.Data.Repository.Implementation.Pipeline;
using Dmb.Crm.Data.Repository.Implementation.Tag;
using Dmb.Crm.Data.Repository.Interface.Auth;
using Dmb.Crm.Data.Repository.Interface.Company;
using Dmb.Crm.Data.Repository.Interface.Contact;
using Dmb.Crm.Data.Repository.Interface.Location;
using Dmb.Crm.Data.Repository.Interface.Opportunity;
using Dmb.Crm.Data.Repository.Interface.Pipeline;
using Dmb.Crm.Data.Repository.Interface.Tag;
using Dmb.Crm.Model.Abstractions;
using Dmb.Crm.Service.Implementation.Auth;
using Dmb.Crm.Service.Implementation.Company;
using Dmb.Crm.Service.Implementation.Contact;
using Dmb.Crm.Service.Implementation.Email;
using Dmb.Crm.Service.Implementation.Location;
using Dmb.Crm.Service.Implementation.Opportunity;
using Dmb.Crm.Service.Implementation.Pipeline;
using Dmb.Crm.Service.Implementation.Tag;
using Dmb.Crm.Service.Interface.Auth;
using Dmb.Crm.Service.Interface.Company;
using Dmb.Crm.Service.Interface.Contact;
using Dmb.Crm.Service.Interface.Location;
using Dmb.Crm.Service.Interface.Opportunity;
using Dmb.Crm.Service.Interface.Pipeline;
using Dmb.Crm.Service.Interface.Tag;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

#region Database
builder.Services.AddDbContext<CrmContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CrmDb")));
#endregion

#region JWT
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "dmbcrm";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "dmbcrm";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
#endregion

#region CORS
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? ["http://localhost:3000"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("CrmWeb", policy =>
        policy.SetIsOriginAllowed(origin =>
            {
                if (corsOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }

                return Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                    && (uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                        || uri.Host.Equals("dmbwebsolutions.com", StringComparison.OrdinalIgnoreCase)
                        || uri.Host.Equals("www.dmbwebsolutions.com", StringComparison.OrdinalIgnoreCase));
            })
            .AllowAnyHeader()
            .AllowAnyMethod());
});
#endregion

#region AutoMapper + cache
builder.Services.AddAutoMapper(typeof(CrmMappingProfile));
builder.Services.AddMemoryCache();
#endregion

#region Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPipelineRepository, PipelineRepository>();
builder.Services.AddScoped<IOpportunityRepository, OpportunityRepository>();
#endregion

#region Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IPipelineService, PipelineService>();
builder.Services.AddScoped<IOpportunityService, OpportunityService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IActivationEmailSender>(sp => sp.GetRequiredService<EmailService>());
builder.Services.AddScoped<IPasswordResetEmailSender>(sp => sp.GetRequiredService<EmailService>());
builder.Services.AddScoped<IExternalLoginEmailSender>(sp => sp.GetRequiredService<EmailService>());
builder.Services.AddHttpClient<IExternalAuthService, ExternalAuthService>();
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
builder.Services.AddScoped<LocationContextFilter>();
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DMB CRM API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CrmWeb");
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var jti = context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;
        if (!string.IsNullOrWhiteSpace(jti))
        {
            var authService = context.RequestServices.GetRequiredService<IAuthService>();
            if (await authService.IsJtiRevokedAsync(jti, context.RequestAborted))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }
    }

    await next();
});

app.MapControllers();
app.MapGet("/", () => Results.Ok(new { status = "ok" }));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
