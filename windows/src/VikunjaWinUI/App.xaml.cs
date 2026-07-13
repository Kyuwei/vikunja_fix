using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using VikunjaWinUI.Services;
using VikunjaWinUI.ViewModels;

namespace VikunjaWinUI;

/// <summary>
/// Application entry point. Owns the dependency-injection container and the
/// single main window. Services are resolved through <see cref="GetService{T}"/>
/// because WinUI instantiates XAML pages with a parameterless constructor, so a
/// lightweight service-locator bridge is the idiomatic way to hand them their
/// view models.
/// </summary>
public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
    }

    public static new App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    public Window? MainWindow => _window;

    public static T GetService<T>() where T : class
        => Current.Services.GetRequiredService<T>();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Settings + secure token storage back the session that every API call
        // depends on, so they are singletons.
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ICredentialStore, CredentialStore>();
        services.AddSingleton<AuthSession>();

        // A single pooled HttpClient for the whole app. The base address is set
        // per-request from AuthSession because the server URL is user-supplied.
        services.AddHttpClient<IVikunjaApiClient, VikunjaApiClient>();

        // View models are transient — a fresh instance per navigation.
        services.AddTransient<LoginViewModel>();
        services.AddTransient<TaskListViewModel>();

        return services.BuildServiceProvider();
    }
}
