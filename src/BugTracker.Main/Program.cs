using GihanSoft.Framework.Web.Bootstrap.ConditionalPipelineUse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler(opt => opt.ExceptionHandlingPath = "/error");

var app = builder.Build();

var isDevelopment = app.Environment.IsDevelopment();

_ = app
    .If(isDevelopment, a => a.UseDeveloperExceptionPage())
    .If(!isDevelopment, a => a.UseExceptionHandler())
    .If(!isDevelopment, a => a.UseHsts())
    .UseHttpsRedirection()
    ;

app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => "There is an error!");

app.Run();
