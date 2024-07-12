using BugTracker.Main.Features.Identity.Data;

using GihanSoft.Framework.Web.Bootstrap.Initialization;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BugTracker.Main.Features.Identity.Startup;

internal static class IdentityServices
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services);
        AddAuthentication(services);
        AddAuthorization(services);
        AddAspIdentity(services);
        AddBlazorDependencies(services);
        AddInitializers(services, configuration);

        return services;
    }

    private static ValueTuple AddDbContext(IServiceCollection services)
    {
        services.AddDbContext<IdentityDbContext>((sp, builder) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var environment = sp.GetRequiredService<IHostEnvironment>();

            var connectionString = configuration.GetConnectionString("identity")
                ?? configuration.GetConnectionString("default");
            builder.UseNpgsql(connectionString, opt =>
            {
                opt.MigrationsHistoryTable("__ef_migrations_history", "identity");
            });

            builder.UseSnakeCaseNamingConvention();
            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            builder.ConfigureWarnings(opt =>
                opt.Log([
                    (CoreEventId.FirstWithoutOrderByAndFilterWarning, LogLevel.Warning),
                    (CoreEventId.RowLimitingOperationWithoutOrderByWarning, LogLevel.Warning),
                    (CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning, LogLevel.Warning),
                ]));

            if (environment.IsDevelopment())
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
            }
        });
        return default;
    }

    private static ValueTuple AddAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
        }).AddIdentityCookies(builder =>
        {
            builder.ApplicationCookie?.Configure(options =>
            {
                options.LoginPath = "/Identity/SignIn";
                options.LogoutPath = "/Identity/SignOut";
                options.AccessDeniedPath = "/Identity/AccessDenied";
            });
        });

        return default;
    }

    private static ValueTuple AddAuthorization(IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthorizationBuilder()
            .AddPolicy("sarvar", builder => builder.RequireUserName("sarvar"));

        return default;
    }

    private static ValueTuple AddAspIdentity(IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>(
            options =>
            {
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.UserId;
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.UserName;
                options.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
                options.ClaimsIdentity.SecurityStampClaimType = ClaimTypes.SecurityStamp;
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.SecurityStamp;

                options.Password.RequiredLength = 8;

                options.SignIn.RequireConfirmedAccount = false; //TODO: change after implementing Email sender

                options.Stores.ProtectPersonalData = false;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            }).AddEntityFrameworkStores<IdentityDbContext>()
            .AddSignInManager()
            .AddUserManager<AspNetUserManager<AppUser>>()
            ;

        return default;
    }

    private static ValueTuple AddBlazorDependencies(IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        services.AddCascadingAuthenticationState();

        return default;
    }

    private static ValueTuple AddInitializers(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SarvarOptions>(configuration.GetRequiredSection("sarvar"));
        services.AddInitializer<SarvarAccountInitializer>();

        return default;
    }
}
