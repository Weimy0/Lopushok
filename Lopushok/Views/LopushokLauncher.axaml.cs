using Avalonia.Controls;
using Lopushok.ViewModels;

namespace Lopushok.Views
{
    public partial class LopushokLauncher : Window
    {
        public LopushokLauncher()
        {
            InitializeComponent();
            LopushokLauncherViewModel.lopushokLauncher = this;
            DataContext = new LopushokLauncherViewModel();
        }
    }
}
