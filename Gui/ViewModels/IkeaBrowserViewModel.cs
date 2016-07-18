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
using System.Windows;

namespace Gui.ViewModels
{
    public class IkeaBrowserViewModel : INotifyPropertyChanged
    {
        private IIkeaRepository _repositiry;
        private IParser _parser;
        
        private List<Department> _departments;
        private List<SubCategory> _subcategories;
        private List<Product> _products;
        private Department _selectedDepartment;
        private SubCategory _selectedSubCategory;
        private Product _selectedProduct;

        private ParseProgress _itemsCount = new ParseProgress();
        private bool _isBusy = false;

        public IkeaBrowserViewModel(IIkeaRepository repository, IParser parser)
        {
            _repositiry = repository;
            _parser = parser;

            LoadFromSite = new Command(OnLoadFromSite, CanLoadFromSite);
            LoadFromDB = new Command(OnLoadFromDB, CanLoadFromDB);
            Save = new Command(OnSave, CanSave);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Department> Departments
        {
            get { return _departments; }
            private set
            {
                _departments = value;
                NotifyPropertyChanged();
            }
        }

        public List<SubCategory> SubCategories
        {
            get
            {
                if (SelectedDepartment == null)
                {
                    return null;
                }
                return SelectedDepartment.SubCategories;
            }
            private set
            {
                _subcategories = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        public Department SelectedDepartment {
            get { return _selectedDepartment; }
            set
            {
                _selectedDepartment = value;
                NotifyPropertyChanged("SubCategories");
            }
        }

        public SubCategory SelectedSubCategory
        {
            get { return _selectedSubCategory; }
            set
            {
                _selectedSubCategory = value;
                NotifyPropertyChanged("Products");
            }
        }

        public Product SelectedProduct {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyPropertyChanged();
            }
        }
                
        public ParseProgress ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                _itemsCount = value;
                NotifyPropertyChanged();
            }
        }

        public Command LoadFromSite { get; private set; }

        public Command LoadFromDB { get; private set; }

        public Command Save { get; private set; }

        private bool CanLoadFromSite()
        {
            return !_isBusy;
        }

        private async void OnLoadFromSite()
        {
            if (_isBusy)
                return;

            _isBusy = true;
            LoadFromSite.RaiseCanExecuteChanged();
            LoadFromDB.RaiseCanExecuteChanged();
            Save.RaiseCanExecuteChanged();

            await _parser.ParseAsync(new Progress<ParseProgress>(p =>
            {
                ItemsCount.Departments = p.Departments;
                ItemsCount.SubCategories = p.SubCategories;
                ItemsCount.Products = p.Products;
                NotifyPropertyChanged("ItemsCount");
            }));

            _isBusy = false;
            Departments = _parser.Departments;
            SubCategories = _parser.SubCategories;
            Products = _parser.Products;
            
            LoadFromSite.RaiseCanExecuteChanged();
            LoadFromDB.RaiseCanExecuteChanged();
            Save.RaiseCanExecuteChanged();

            MessageBox.Show("Загружено с сайта");
        }
        
        private bool CanLoadFromDB()
        {
            return !_isBusy;
        }

        private async void OnLoadFromDB()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            LoadFromSite.RaiseCanExecuteChanged();
            LoadFromDB.RaiseCanExecuteChanged();
            Save.RaiseCanExecuteChanged();

            await Task.Run(() =>
            {
                _products = _repositiry.GetProducts();
                _subcategories = _repositiry.GetSubCategories();
                Departments = _repositiry.GetDepartments();
            });

            _isBusy = false;
            LoadFromSite.RaiseCanExecuteChanged();
            LoadFromDB.RaiseCanExecuteChanged();
            Save.RaiseCanExecuteChanged();
            ItemsCount.Departments = Departments.Count;
            ItemsCount.SubCategories = _subcategories.Count;
            ItemsCount.Products = _products.Count;
            NotifyPropertyChanged("ItemsCount");

            MessageBox.Show("Загружено из БД");
        }

        private bool CanSave()
        {
            return (!_isBusy && _products != null);
        }

        private async void OnSave()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            LoadFromSite.RaiseCanExecuteChanged();
            LoadFromDB.RaiseCanExecuteChanged();
            Save.RaiseCanExecuteChanged();

            await Task.Run(() =>
            {
                _repositiry.Clear();
                _repositiry.AddDepartments(Departments);
                _repositiry.AddSubCategories(_subcategories);
                _repositiry.AddProducts(_products);
                _repositiry.Save();
            });

            _isBusy = false;
            LoadFromSite.RaiseCanExecuteChanged();
            LoadFromDB.RaiseCanExecuteChanged();
            Save.RaiseCanExecuteChanged();

            MessageBox.Show("Сохранено в БД");
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
