using Microsoft.UI.Xaml;
using ScoopX.Services;

namespace ScoopX
{
    public partial class App : Application
    {
        private Window? _window;

        public static Window? MainWindow { get; private set; }

        public App()
        {
            // Application.RequestedTheme must be set before InitializeComponent.
            ThemeService.Instance.ApplyApplicationThemeBeforeInit(this);
            LanguageService.Instance.ApplyBeforeUi();
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ToastService.Initialize();
            _window = new MainWindow();
            MainWindow = _window;
            ThemeService.Instance.Initialize(_window.DispatcherQueue);
            _window.Activate();
        }
    }
}
