using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lopushok.ViewModels
{
    public class AddAndEditProductViewModel : ViewModelBase
    {
        private string _productTitle = null!;
        private List<MaterialCost> _materiaForProductList = new();
        private string _selectedMaterialForProduct = null!;
        private List<string> _materialList = new();
        private string _selectedMaterial = null!;
        private string _countText = null!;
        private double? Count = null!;

        #region Свойства
        public string ProductTitle
        {
            get => _productTitle;
            set
            {
                _productTitle = value;
                OnPropertyChanged(nameof(ProductTitle));
            }
        }

        public List<MaterialCost> MaterialForProductList
        {
            get { return _materiaForProductList; }
            set
            {
                _materiaForProductList = value;
                OnPropertyChanged(nameof(MaterialForProductList));
            }
        }

        public string SelectedMaterialForProduct
        {
            get => _selectedMaterialForProduct;
            set
            {
                _selectedMaterialForProduct = value;
                OnPropertyChanged(nameof(SelectedMaterialForProduct));
            }
        }

        public List<string> MaterialList
        {
            get
            {
                var list = new LopushokContext().Materials.ToList();
                foreach (var item in list)
                {
                    _materialList.Add(item.Title);
                }
                return _materialList;
            }
            set { _materialList = value; }
        }

        public string SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
            }
        }

        public string CountText
        {
            get => _countText;
            set
            {
                _countText = value;
                if (value != "")
                    ConvertCount(value);
                OnPropertyChanged(nameof(CountText));
            }
        }
        #endregion

        private void ConvertCount(string str)
        {
            int num;
            bool isNum = int.TryParse(str, out num);
            if (isNum)
                Count = Convert.ToDouble(num);
        }

        public AddAndEditProductViewModel()
        {
            
        }

        public AddAndEditProductViewModel(Item selectedItem)
        {
            ProductTitle = selectedItem.Title;
            MaterialForProductList = GetMaterialCosts(new LopushokContext().Products.FirstOrDefault(p => p.Title == selectedItem.Title)!.Id);
        }

        private void AddMaterial()
        {
            if (SelectedMaterial != null && Count != null)
            {
                //MaterialForProductList.Add(new MaterialCost("", Count));
            }
        }


        private List<MaterialCost> GetMaterialCosts(int productId)
        {
            List<MaterialCost> materialCostList = new List<MaterialCost>();
            var productMaterials = new LopushokContext()
                .ProductMaterials
                .Include(m => m.Material)
                .Where(p => p.ProductId == productId)
                .ToList();

            if (productMaterials.Count() == 0)
            {
                return null!;
            }

            foreach (var item in productMaterials)
            {
                materialCostList.Add(new MaterialCost(item.Material.Title, item.Count));
            }
            return materialCostList;
        }
    }

    public class MaterialCost
    {
        public string Title { get; set; } = null!;
        public double? Count { get; set; }

        public MaterialCost(string title, double? count)
        {
            Title = title;
            Count = count;
        }
    }
}
