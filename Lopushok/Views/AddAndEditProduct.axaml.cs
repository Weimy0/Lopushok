using Avalonia.Controls;
using Lopushok.ViewModels;

namespace Lopushok.Views
{
    public partial class AddAndEditProduct : Window
    {
        public AddAndEditProduct()
        {
            InitializeComponent();
            DataContext = new AddAndEditProductViewModel();
        }

        public AddAndEditProduct(Item selectedItem)
        {
            InitializeComponent();
            DataContext = new AddAndEditProductViewModel(selectedItem);
        }
    }
}
