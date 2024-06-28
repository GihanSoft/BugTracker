using BugTracker.Main.Components;

using GihanSoft.Framework.Web.Bootstrap.ConditionalPipelineUse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

var app = builder.Build();

var isDevelopment = app.Environment.IsDevelopment();

_ = app
    .If(isDevelopment, a => a.UseDeveloperExceptionPage())
    .If(!isDevelopment, a => a.UseExceptionHandler("/500"))
    .If(!isDevelopment, a => a.UseHsts())
    .UseHttpsRedirection()
    .UseStatusCodePagesWithReExecute("/{0}")
    .UseStaticFiles()
    .UseAntiforgery()
    ;

app.MapRazorComponents<App>();

app.Run();
