using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============ CORS DEV: chấp mọi origin (kể cả 'null') ============
const string PermissiveDevCors = "PermissiveDevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(PermissiveDevCors, policy =>
        policy
            // CHẤP MỌI ORIGIN (kể cả 'null' khi mở file://)
            .SetIsOriginAllowed(origin => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
    // Nếu bạn dùng cookie/session (withCredentials) thì mở dòng dưới:
    // .AllowCredentials()
    );
});

// ============ Controllers / Swagger ============
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============ JWT ============
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false; // DEV
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ❶ CORS phải đứng TRƯỚC auth/authorization & MapControllers
app.UseCors(PermissiveDevCors);

// (Tuỳ chọn) “Bảo hiểm” cho preflight nếu hosting/proxy nào đó nuốt mất CORS.
// Có thể bỏ nếu bạn đã thấy OPTIONS trả đúng header CORS.
app.Use(async (ctx, next) =>
{
    if (string.Equals(ctx.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
    {
        var origin = ctx.Request.Headers["Origin"].ToString();
        if (!string.IsNullOrEmpty(origin) || origin == "null")
        {
            ctx.Response.Headers["Access-Control-Allow-Origin"] = string.IsNullOrEmpty(origin) ? "null" : origin;
            ctx.Response.Headers["Vary"] = "Origin";
        }
        ctx.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,PATCH,DELETE,OPTIONS";
        ctx.Response.Headers["Access-Control-Allow-Headers"] = "Authorization,Content-Type,Accept";
        // Nếu dùng cookie:
        // ctx.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        ctx.Response.StatusCode = StatusCodes.Status204NoContent;
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
