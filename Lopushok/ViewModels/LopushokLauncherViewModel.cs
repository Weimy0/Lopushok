using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Lopushok.Models;
using Lopushok.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lopushok.ViewModels
{
    public class LopushokLauncherViewModel : ViewModelBase
    {
        public static LopushokLauncher lopushokLauncher = null!;
        private List<Item> ProductList = new();
        private List<Item> _items = new();
        private Item _selectedItem = null!;
        private string _search = null!;
        private List<string> _sortingList = new();
        private List<string> _filteringList = new();
        private string _selectedSorting = null!;
        private string _selectedFiltering = null!;
        private int CountPage;
        private int SelectedPage;

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

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                AddAndEditProduct addAndEditProduct = new AddAndEditProduct(SelectedItem);
                addAndEditProduct.ShowDialog(lopushokLauncher);
                ProductList = GetDataBaseItems();
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                GetFilteringBase(SelectedFiltering);
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
                if (value != null)
                    GetSortingBase(ProductList, value);
                OnPropertyChanged(nameof(SelectedSorting));
            }
        }

        public string SelectedFiltering
        {
            get => _selectedFiltering;
            set
            {
                _selectedFiltering = value;
                if (value != null)
                {
                    GetFilteringBase(value);
                }

                OnPropertyChanged(nameof(SelectedFiltering));
            }
        }
        #endregion

        public LopushokLauncherViewModel()
        {
            ProductList = GetDataBaseItems();
            CreateSortingAndFilteringList();
            CountPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ProductList.Count) / 4));
            SelectedPage = 1;
            GetItemsForPage();
        }

        private void GetItemsForPage()
        {
            List<Item> items = new List<Item>();
            int maxItem = SelectedPage * 4;
            int minItem = SelectedPage * 4 - 4;
            for (; minItem < maxItem && minItem < ProductList.Count(); minItem++)
            {
                items.Add(ProductList[minItem]);
            }
            CountPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ProductList.Count() / 4)));
            Items = items;
            CreateButtonPage(SelectedPage - 1);
        }

        public void CreateButtonPage(int startPage)
        {
            lopushokLauncher.StackPanelNumberPage.Children.Clear();
            int endPage = startPage + 3;
            if (startPage == 0)
            {
                startPage++;
                endPage++;
            }

            if (startPage + 3 > CountPage && 3 < CountPage)
            {
                startPage = CountPage - 3;
            }

            if (CountPage < 4)
            {
                startPage = 1;
            }

            if (CountPage != 0)
            {
                Button buttonLeft = new Button();
                buttonLeft.Content = "<";
                buttonLeft.Click += ButtonLeft_Click;
                lopushokLauncher.StackPanelNumberPage.Children.Add(buttonLeft);

                for (; startPage <= endPage && startPage <= CountPage; startPage++)
                {
                    Button buttonNumberPage = new Button();
                    buttonNumberPage.Content = startPage;
                    buttonNumberPage.Click += ButtonNumberPage_Click;
                    if (startPage == SelectedPage)
                    {
                        buttonNumberPage.BorderBrush = new SolidColorBrush(Colors.Black);

                    }
                    lopushokLauncher.StackPanelNumberPage.Children.Add(buttonNumberPage);
                }

                Button buttonRight = new Button();
                buttonRight.Content = ">";
                buttonRight.Click += ButtonRight_Click;
                lopushokLauncher.StackPanelNumberPage.Children.Add(buttonRight);
            }
        }

        private void ButtonLeft_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (SelectedPage - 1 > 0)
            {
                SelectedPage -= 1;
                GetItemsForPage();
            }
        }

        private void ButtonNumberPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var button = (sender as Button);
            SelectedPage = Convert.ToInt32(button!.Content);
            GetItemsForPage();
        }

        private void ButtonRight_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (SelectedPage + 1 <= CountPage)
            {
                SelectedPage += 1;
                GetItemsForPage();
            }
        }

        private void GetSearchBase(string search)
        {
            ProductList = ProductList
                .Where(s => s.Title.ToLower().Contains(search.ToLower()))
                .ToList();
            GetSortingBase(ProductList, SelectedFiltering);
        }

        private void GetSortingBase(List<Item> items, string sort)
        {
            if (sort == "Без сортировки")
            {
                SelectedSorting = null!;
                ProductList = GetDataBaseItems()
                    .Where(item => items.Any(t => t.Title == item.Title))
                    .ToList();
            }
            else if (sort == "По названию")
            {
                ProductList = items.OrderBy(t => t.Title).ToList();
            }
            else if (sort == "По типу")
            {
                ProductList = items.OrderBy(t => t.Type).ToList();
            }
            else if (sort == "По стоимости")
            {
                ProductList = items.OrderBy(t => t.Cost).ToList();
            }
            else if (sort == "По артиклу")
            {
                ProductList = items.OrderBy(t => t.ArticleNumber).ToList();
            }
            SelectedPage = 1;
            GetItemsForPage();
        }

        private void GetFilteringBase(string filtr)
        {
            if (filtr == "Без фильтрации" || filtr == null)
            {
                SelectedFiltering = null!;
                ProductList = GetDataBaseItems();
            }
            else if (filtr == "С материалами")
            {
                ProductList = GetDataBaseItems()
                    .Where(f => f.Materials != null)
                    .ToList();
            }
            else if (filtr == "Без материалов")
            {
                ProductList = GetDataBaseItems()
                    .Where(f => f.Materials == null || f.Materials == "")
                    .ToList();
            }
            else
            {
                ProductList = GetDataBaseItems()
                    .Where(f => f.Type == filtr)
                    .ToList();
            }

            if (Search != null)
            {
                GetSearchBase(Search);
            }
            GetSortingBase(ProductList, SelectedSorting);
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
                var image = GetImage(product.Image!);
                var item = new Item(product.Title, product.ProductType!.Title,
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
}