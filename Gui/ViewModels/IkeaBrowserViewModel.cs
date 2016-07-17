using DataAccess;
using IkeaParser;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public class IkeaBrowserViewModel : INotifyPropertyChanged
    {
        private IkeaRepository _rep = new IkeaRepository();

        private Parser _parser;
        private List<SubCategory> _subcategories = new List<SubCategory>();
        private List<Product> _products = new List<Product>();
        private Department _selectedDepartment;
        private SubCategory _selectedSubCategory;
        private Product _selectedProduct;

        public List<Department> Departments { get; private set; }

        public Product SelectedProduct {
            get
            {
                return _selectedProduct;
            }
            set
            {
                _selectedProduct = value;
                NotifyPropertyChanged();
            }
        }

        public Department SelectedDepartment {
            get
            {
                return _selectedDepartment;
            }
            set
            {
                _selectedDepartment = value;
                NotifyPropertyChanged("SubCategories");
            }
        }

        public SubCategory SelectedSubCategory
        {
            get
            {
                return _selectedSubCategory;
            }
            set
            {
                _selectedSubCategory = value;
                NotifyPropertyChanged("Products");
            }
        }

        public List<SubCategory> SubCategories {
            get
            {
                if (SelectedDepartment == null)
                    return null;
                return SelectedDepartment.SubCategories;
            }
            private set
            {
                _subcategories = value;
            }
        }

        public List<Product> Products
        {
            get
            {
                if (SelectedSubCategory == null)
                {
                    return null;
                }
                return SelectedSubCategory.Products;
            }
            private set
            {
                _products = value;
            }
        }

        public Command LoadBase { get; private set; }

        public Command Test { get; private set; }

        public Command Save { get; private set; }

        private bool _isLoading = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool CanLoadBase()
        {
            return !_isLoading;
        }

        private async void OnLoadBase()
        {
            _isLoading = true;
            LoadBase.RaiseCanExecuteChanged();
            await _parser.ParseAsync();
            Departments = _parser.Departments;
            SubCategories = _parser.Subcategories;
            Products = _parser.Products;
            NotifyPropertyChanged("Departments");
            _isLoading = false;
            LoadBase.RaiseCanExecuteChanged();
        }

        public IkeaBrowserViewModel()
        {
        
            LoadBase = new Command(OnLoadBase, CanLoadBase);
            Test = new Command(OnTest);
            Save = new Command(OnSave, CanSave);
            _parser = new Parser();

            LoadData();
            
            //_products.Add(new Product { Id = "1", Image = "http://www.ikea.com/ru/ru/images/products/tostero-cehol-dla-sezlonga-cernyj__0306790_PE427288_S4.JPG", Name = "ТОСТЕРО" });
            //_products.Add(new Product { Id = "2", Image = "http://www.ikea.com/ru/ru/images/products/bussen-puf-mesok-d-doma-sada-oranzevyj__0390218_PE559785_S4.JPG", Name = "БУССЭН" });

        }

        private async void LoadData()
        {
            await Task.Run(() =>
            {
                _products = _rep.GetProducts();
                _subcategories = _rep.GetSubCategories();
                Departments = _rep.GetDepartments();
            });
            NotifyPropertyChanged("Departments");
        }

        private bool CanSave()
        {
            return _products != null;
        }

        private void OnSave()
        {
            _rep.Clear();
            _rep.AddDepartments(Departments);
            _rep.AddSubCategories(_subcategories);
            _rep.AddProducts(_products);
            _rep.Save();
        }

        private void OnTest()
        {
            
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
