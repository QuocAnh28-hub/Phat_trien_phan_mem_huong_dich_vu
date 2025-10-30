using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// 1️⃣ Đọc file cấu hình ocelot.json
// ---------------------------------------------
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ---------------------------------------------
// 2️⃣ Thêm các service cần thiết
// ---------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------
// 3️⃣ Thêm cấu hình CORS cho Gateway
// ---------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5500", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "https://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ---------------------------------------------
// 4️⃣ Thêm Ocelot
// ---------------------------------------------
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// ---------------------------------------------
// 5️⃣ Cấu hình pipeline
// ---------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚠️ CORS phải đặt trước Ocelot
app.UseCors("AllowLocalhost5500");

app.UseAuthorization();

// ⚡ Ocelot Middleware (bắt buộc phải có "await")
await app.UseOcelot();

app.Run();
