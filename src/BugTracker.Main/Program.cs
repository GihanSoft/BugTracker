using BugTracker.Main;
using BugTracker.Main.Common.UI;
using BugTracker.Main.Features.Backlog.Startup;
using BugTracker.Main.Features.Identity.Startup;

using GihanSoft.Framework.Web.Bootstrap.ConditionalPipelineUse;
using GihanSoft.Framework.Web.Bootstrap.Initialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<ForwardedHeadersOptions>(builder.Configuration.GetSection(nameof(ForwardedHeadersOptions)));

builder.Services.AddRazorComponents();
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddBacklogService();

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

app.MapDefaultEndpoints();

app.MapRazorComponents<App>();

await app.RunInitializersAsync();

app.Run();
