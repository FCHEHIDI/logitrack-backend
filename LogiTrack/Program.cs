// Usings
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;
using LogiTrack.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

// Only one builder declaration
var builder = WebApplication.CreateBuilder(args);

// Add health checks
builder.Services.AddHealthChecks();
// Add logging configuration
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("AuthPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }
        ));
});
// Force the app to listen on a specific port regardless of launchSettings.json
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LogiTrack API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<LogiTrackContext>(options =>
    options.UseSqlite("Data Source=logitrack.db"));
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LogiTrackContext>()
    .AddDefaultTokenProviders();

// Only one app declaration
var app = builder.Build();

// Use rate limiting middleware
app.UseRateLimiter();

// Global error handler for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (error != null)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(error.Error, "Unhandled exception");
            }
            await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred.\"}");
        });
    });
}

// Health check endpoint
app.MapHealthChecks("/health");

// Root endpoint to avoid 404 on '/'
// (Place after 'var app = builder.Build();')



// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("AuthPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }
        ));
});
// Force the app to listen on a specific port regardless of launchSettings.json
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LogiTrack API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<LogiTrackContext>(options =>
    options.UseSqlite("Data Source=logitrack.db"));

//Add Memory Cache
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LogiTrackContext>()
    .AddDefaultTokenProviders();



// Use rate limiting middleware
app.UseRateLimiter();

// Global error handler for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (error != null)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(error.Error, "Unhandled exception");
            }
            await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred.\"}");
        });
    });
}

// Root endpoint to avoid 404 on '/'
app.MapGet("/", () => "LogiTrack API is running. Try /test-inventory or /test-order");

// Test controller endpoint for InventoryItem example

app.MapGet("/test-inventory", () => {
    var item = new InventoryItem {
        ItemId = 1,
        Name = "Pallet Jack",
        Quantity = 12,
        Location = "Warehouse A"
    };
    // Return the formatted string as in DisplayInfo
    return $"Item: {item.Name} | Quantity: {item.Quantity} | Location: {item.Location}";
});


// Test block: create an order, add/remove items, and print the summary
app.MapGet("/test-order", () =>
{
    var item1 = new InventoryItem
    {
        ItemId = 1,
        Name = "Pallet Jack",
        Quantity = 12,
        Location = "Warehouse A"
    };
    var item2 = new InventoryItem
    {
        ItemId = 2,
        Name = "Hand Truck",
        Quantity = 5,
        Location = "Warehouse B"
    };
    var order = new LogiTrack.Models.Order
    {
        OrderId = 1001,
        CustomerName = "Samir",
        DatePlaced = new DateTime(2025, 4, 5)
    };
    order.AddItem(item1);
    order.AddItem(item2);
    order.RemoveItem(item2); // Remove one item
    using (var sw = new System.IO.StringWriter())
    {
        Console.SetOut(sw);
        order.GetOrderSummary();
        return sw.ToString().Trim();
    }
});
// Health check endpoint
app.MapHealthChecks("/health");

// Configure the database context

// Seed data and print inventory at startup
void SeedAndPrintInventory(LogiTrackContext context)
{
    context.Database.EnsureCreated();
    if (!context.InventoryItems.Any())
    {
        context.InventoryItems.Add(new InventoryItem
        {
            Name = "Pallet Jack",
            Quantity = 12,
            Location = "Warehouse A"
        });
        context.SaveChanges();
    }
    var items = context.InventoryItems.ToList();
    foreach (var item in items)
    {
        // Outcome: Should print to console on first run:
        // Item: Pallet Jack | Quantity: 12 | Location: Warehouse A
        item.DisplayInfo();
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();
    SeedAndPrintInventory(context);

    // Seed roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!roleManager.RoleExistsAsync("User").GetAwaiter().GetResult())
    {
        roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();
    }
    // Add more roles if needed
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply rate limiting to auth endpoints
app.MapControllers().RequireRateLimiting("AuthPolicy");
app.Run();

