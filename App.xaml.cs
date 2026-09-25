using Microsoft.UI.Xaml;

namespace ScoopX
{
    public partial class App : Application
    {
        private Window? _window;

        public static Window? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindow = _window;
            _window.Activate();
        }
    }
}
