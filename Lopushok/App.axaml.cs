using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Lopushok.ViewModels;
using Lopushok.Views;

namespace Lopushok
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new LopushokLauncher();
                {
                    DataContext = new LopushokLauncherViewModel();
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}