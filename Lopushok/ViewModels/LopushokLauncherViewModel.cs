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
        private List<Item> _items = new();
        private string _search = null!;
        private List<string> _sortingList = new();
        private List<string> _filteringList = new();
        private string _selectedSorting = null!;
        private string _selectedFiltering = null!;

        #region Свойства
        public List<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                GetSearchBase(value);
                OnPropertyChanged(nameof(Search));
            }
        }

        public List<string> SortingList
        {
            get => _sortingList;
            set
            {
                _sortingList = value;
                OnPropertyChanged(nameof(SortingList));
            }
        }

        public List<string> FilteringList
        {
            get { return _filteringList; }
            set
            {
                _filteringList = value;
                OnPropertyChanged(nameof(FilteringList));
            }
        }

        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                _selectedSorting = value;
                if (value != "")
                    GetSortingBase(Items, value);
                OnPropertyChanged(nameof(SelectedSorting));
            }
        }

        public string SelectedFiltering
        {
            get => _selectedFiltering;
            set
            {
                _selectedFiltering = value;
                if (value != "")
                    GetFilteringBase(value);
                OnPropertyChanged(nameof(SelectedFiltering));
            }
        }
        #endregion

        public LopushokLauncherViewModel()
        {
            Items = GetDataBaseItems();
            CreateSortingAndFilteringList();
        }

        private void GetSearchBase(string search)
        {
            if (Items.Count() == 0)
            {
                GetFilteringBase(SelectedFiltering);
            }
            Items = Items
                .Where(s => s.Title.ToLower().Contains(search.ToLower()))
                .ToList();
            GetSortingBase(Items, SelectedSorting);
        }

        private void GetFilteringBase(string filtr)
        {
            if (filtr == "Без фильтрации" || filtr == null)
            {
                SelectedFiltering = "";
                Items = GetDataBaseItems();
            }
            else if (filtr == "С материалами")
            {
                Items = GetDataBaseItems()
                    .Where(f => f.Materials != null)
                    .ToList();
            }
            else if (filtr == "Без материалов")
            {
                Items = GetDataBaseItems()
                    .Where(f => f.Materials == null)
                    .ToList();
            }
            else
            {
                Items = GetDataBaseItems()
                    .Where(f => f.Type == filtr)
                    .ToList();
            }
            GetSortingBase(Items, SelectedSorting);
        }

        private void GetSortingBase(List<Item> items, string sort)
        {
            if (sort == "Без сортировки")
            {
                SelectedSorting = "";
                Items = GetDataBaseItems()
                    .Where(item => items.Any(t => t.Title == item.Title))
                    .ToList();
            }
            else if (sort == "По названию")
            {
                Items = items.OrderBy(t => t.Title).ToList();
            }
            else if (sort == "По типу")
            {
                Items = items.OrderBy(t => t.Type).ToList();
            }
            else if (sort == "По стоимости")
            {
                Items = items.OrderBy(t => t.Cost).ToList();
            }
            else if (sort == "По артиклу")
            {
                Items = items.OrderBy(t => t.ArticleNumber).ToList();
            }
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
                decimal cost = GetCost(product.Id);
                var image = GetImage(product.Image);
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

            if (productMaterials.Count() == 0)
                return null;

            sb.Append("Материалы: ");

            foreach (var pm in productMaterials)
                sb.Append($"{pm.Material.Title}, ");

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        private decimal GetCost(int productId)
        {
            var productMaterials = new LopushokContext()
                .ProductMaterials
                .Include(m => m.Material)
                .Where(p => p.ProductId == productId)
                .ToList();

            decimal cost = 0;
            foreach (var pm in productMaterials)
            {
                cost += Convert.ToDecimal(pm.Count) * pm.Material.Cost;
            }

            return cost;
        }

        private Bitmap GetImage(string image)
        {
            if (image == "")
                image = @"\products\picture.png";
            return new Bitmap("." + image);
        }

        private void CreateSortingAndFilteringList()
        {
            SortingList.Add("Без сортировки");
            SortingList.Add("По названию");
            SortingList.Add("По типу");
            SortingList.Add("По стоимости");
            SortingList.Add("По артиклу");

            FilteringList.Add("Без фильтрации");
            FilteringList.Add("С материалами");
            FilteringList.Add("Без материалов");
            foreach (var pt in new LopushokContext().ProductTypes)
            {
                FilteringList.Add(pt.Title);
            }
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
