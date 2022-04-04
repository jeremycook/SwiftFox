using Microsoft.AspNetCore.Authentication.Negotiate;
using Swiftfox.Mvc;
using Swiftfox.Startup;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

var assemblies = new[]
{
    typeof(Swiftfox.SwiftfoxOptions).Assembly,
    typeof(Program).Assembly,
};
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate(options =>
   {
       options.Events = new()
       {
           OnAuthenticated = (AuthenticatedContext context) =>
           {
               WindowsPrincipal windowsPrincipal = (WindowsPrincipal)context.Principal!;
               WindowsIdentity windowsIdentity = (WindowsIdentity)windowsPrincipal.Identity!;
               string sid = windowsIdentity.User!.Value;

               // TODO: Lookup database user based on SID.
               // For now we're pretending by hashing the SID into a GUID.
               var user = new
               {
                   UserId = new Guid(MD5.HashData(Encoding.UTF8.GetBytes(sid))),
                   Name = windowsIdentity.Name!.Split('\\').Last(),
                   Username = windowsIdentity.Name!.Split('\\').Last().ToLowerInvariant(),
               };

               var newIdentity = new ClaimsIdentity(authenticationType: windowsIdentity.AuthenticationType, nameType: "name", roleType: "role");
               newIdentity.AddClaim(new("sub", user.UserId.ToString()));
               newIdentity.AddClaim(new("name", user.Name));
               newIdentity.AddClaim(new("username", user.Username));

               context.Principal = new ClaimsPrincipal(newIdentity);

               return Task.CompletedTask;
           }
       };
   });

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});


builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddJsonQueryParameterSupport();
});
builder.Services.AddRazorPages();

builder.Services.ConfigureOptionsFromAssemblies(assemblies, builder.Configuration);
builder.Services.AddServicesFromAssemblies(assemblies);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
