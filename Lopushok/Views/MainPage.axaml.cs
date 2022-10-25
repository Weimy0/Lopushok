using Avalonia.Controls;
using Lopushok.ViewModels;

namespace Lopushok.Views
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            MainPageViewModel.mainPage = this;
            DataContext = new MainPageViewModel();
        }
    }
}
