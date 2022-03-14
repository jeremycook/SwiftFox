using Microsoft.AspNetCore.Authentication.Negotiate;
using SwiftFox.Startup;

var assemblies = new[]
{
    typeof(SwiftFox.SwiftFoxOptions).Assembly,
    typeof(Program).Assembly,
};
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages();

builder.Services.ConfigureOptionsFromAssemblies(assemblies, builder.Configuration);
builder.Services.AddServicesFromAssemblies(assemblies);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
