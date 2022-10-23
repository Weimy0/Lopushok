using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lopushok.Views;
using Lopushok.Models;

namespace Lopushok.ViewModels
{
    public class LopushokLauncherViewModel : ViewModelBase
    {
        public static MainWindow LopushokLauncherWindow = new();
        public List<Item> Items;
        public LopushokLauncherViewModel()
        {
            Items = GetDataBaseItems();
            LopushokLauncherWindow.ProductList.Items = Items; 
        }

        private List<Item> GetDataBaseItems()
        {
            List<Item> items = new List<Item>();
            var products = new LopushokContext()
                .Products
                .Include(p => p.ProductType)
                .ToList();

            foreach (var product in products)
            {
                string? materials = GetMaterials(product.Id);
                var image = GetImage(product.Image);
                decimal cost = 0;
                var item = new Item(product.Title, product.ProductType.Title,
                    product.ArticleNumber, cost, materials, image);

                items.Add(item);
            }
            return items;
        }

        private string? GetMaterials(int productId)
        {
            StringBuilder sb = new StringBuilder();
            var productMaterials = new LopushokContext()
                .ProductMaterials
                .Include(m => m.Material)
                .Where(p => p.ProductId == productId)
                .ToList();

            if (productMaterials == null)
                return null;

            sb.Append("Материалы: ");

            foreach (var pm in productMaterials)
                sb.Append($"{pm.Material.Title}, ");

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        private Bitmap GetImage(string image)
        {
            string path = (image == "") ? "products/picture.png" : image;

            return new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri($"avares://{Assembly.GetEntryAssembly().GetName().Name}/{path}")));
        }
    }

    public class Item
    {
        public string Title { get; set; }
        public string? Type { get; set; }
        public string ArticleNumber { get; set; }
        public decimal Cost { get; set; }
        public string? Material { get; set; }
        public Bitmap Image { get; set; }

        public Item(string title, string? type,
            string articleNumber, decimal cost,
            string? material, Bitmap image)
        {
            Title = title;
            Type = type;
            ArticleNumber = articleNumber;
            Cost = cost;
            Material = material;
            Image = image;
        }
    }
}
