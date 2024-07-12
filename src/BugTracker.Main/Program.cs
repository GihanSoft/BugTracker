using BugTracker.Main.Components;
using BugTracker.Main.Features.Identity.Startup;

using GihanSoft.Framework.Web.Bootstrap.ConditionalPipelineUse;
using GihanSoft.Framework.Web.Bootstrap.Initialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(builder.Configuration.GetSection(nameof(ForwardedHeadersOptions)));

builder.Services.AddRazorComponents();
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

var isDevelopment = app.Environment.IsDevelopment();
_ = app
    .If(isDevelopment, a => a.UseDeveloperExceptionPage())
    .If(!isDevelopment, a => a.UseExceptionHandler("/500"))
    .UseForwardedHeaders()
    .If(!isDevelopment, a => a.UseHsts())
    .UseHttpsRedirection()
    .UseStatusCodePagesWithReExecute("/{0}")
    .UseStaticFiles()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseAntiforgery()
    .UseEndpoints(_ => { })
    ;

app.MapRazorComponents<App>();

await app.RunInitializersAsync();

app.Run();
