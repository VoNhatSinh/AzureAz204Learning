using Azure_Az204.Services;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Logging.AddApplicationInsights(configureTelemetryConfiguration: configuration =>
{
    configuration.ConnectionString = ConnectionStringHelper.GetAppInsightsInstrumentationKey();
}, _ => { });
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Azure_Az204", LogLevel.Information);

builder.Services.AddSignalR();
builder.Services.AddHostedService<ServiceBusListener>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.MapControllers();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapHub<MessageHub>("messageHub");
app.MapHub<EventGridHub>("eventGridHub");


app.Run();
