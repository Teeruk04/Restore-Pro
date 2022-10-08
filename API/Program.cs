using System.Text;
using API.Data;
using API.Entities;
using API.Middleware;
using API.RequestHelpers;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
#region Swagger Config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt auth header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
               Scheme = "Bearer",
               Name = "Bearer",
               In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
#endregion

#region เชื่อมต่อไปยัง heroku Server และใช้ค่าที่ config ไว้แล้วในฝั่ง Heroku
builder.Services.AddDbContext<StoreContext>(options =>
{
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
string connStr;
if (env == "Development")
{
// Use connection string from file.
connStr = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
// Use connection string provided at runtime by Heroku.
var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
// Parse connection URL to connection string for Npgsql
connUrl = connUrl.Replace("postgres://", string.Empty);
var pgUserPass = connUrl.Split("@")[0];
var pgHostPortDb = connUrl.Split("@")[1];
var pgHostPort = pgHostPortDb.Split("/")[0];
var pgDb = pgHostPortDb.Split("/")[1];
var pgUser = pgUserPass.Split(":")[0];
var pgPass = pgUserPass.Split(":")[1];
var pgHost = pgHostPort.Split(":")[0];
var pgPort = pgHostPort.Split(":")[1];
connStr = $"Server={pgHost};Port={pgPort};UserId={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;Trust ServerCertificate=true";
}
// Whether the connection string came from the local development configuration file
// or from the environment variable from Heroku, use it to set up your DbContext.
options.UseNpgsql(connStr);
});
#endregion

#region Cors
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,

    policy =>
    {
        policy.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:3000");
    });

});
//AllowCredentials() อนุญําตให้client ใช้คุกกี้ของ Api ได้
#endregion

#region Identityสร้างเซอร์วิส User,Role (ระวังการเรียงล าดับ)
builder.Services.AddIdentityCore<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<Role>()
.AddEntityFrameworkStores<StoreContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
    .GetBytes(builder.Configuration["JWTSettings:TokenKey"]))
    };
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddScoped<ImageService>();
#endregion

var app = builder.Build();

#region //สร้ํางข้อมูลจ ําลอง Fake data
using var scope = app.Services.CreateScope(); //using หลังท ํางํานเสร็จจะถูกท ําลํายจํากMemory
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    // ช่วยได้เยอะ ต้องใช้
    await context.Database.MigrateAsync(); //สร้ําง DB ให้อัตโนมัติถ้ํายังไม่มี
    await DbInitializer.Initialize(context, userManager); //สร้างข้อมูลสินค้าและยูเซอร์จ าลอง
}
catch (Exception ex)
{
    logger.LogError(ex, "Problem migrating data");
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); ใช้หน้าWeb  API ไม่ใช่
#region ส่ง error ไปให้Axios ตอนท ํา Interceptor
app.UseMiddleware<ExceptionMiddleware>();
#endregion

app.UseRouting(); //ระวังต้องใช้ตัวนี้มิฉนั้นตอน deploy รันไม่ได้

app.UseDefaultFiles(); // อนุญาตให้เรียกไฟล์ต่างๆ ใน wwwroot
app.UseStaticFiles(); // อนุญาตให้เข้าถึงไฟล์ค่าคงที่ได้

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToController("Index", "Fallback");
});

await app.RunAsync();