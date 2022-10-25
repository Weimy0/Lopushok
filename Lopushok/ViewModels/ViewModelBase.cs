using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lopushok.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public static List<Item> AddItems = null!;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null!)
        {
            PropertyChangedEventHandler handler = PropertyChanged!;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Item
    {
        public string Title { get; set; }
        public string? Type { get; set; }
        public string ArticleNumber { get; set; }
        public decimal Cost { get; set; }
        public string? Materials { get; set; }
        public Bitmap Image { get; set; }

        public Item(string title, string? type,
            string articleNumber, decimal cost,
            string? material, Bitmap image)
        {
            Title = title;
            Type = type;
            ArticleNumber = articleNumber;
            Cost = cost;
            Materials = material;
            Image = image;
        }
    }
}
