using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.MessageQueue;
using Planner.Services;
using Planner.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PlannerDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<PlannerDbContext>();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = "506046613602-ndlvtvdihtguvtu8iuu0qh17et973vqd.apps.googleusercontent.com";
        googleOptions.ClientSecret = "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";
    });

builder.Services.AddControllers();
builder.Services.AddRazorPages(options =>
 {
     options.Conventions.AuthorizePage("/Calendar");
 });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IMessageProducer, MessageProducer>();
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddHostedService<MQMonitor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseWebSockets();

app.Run();
