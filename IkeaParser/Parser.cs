using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IkeaParser
{
    public class Parser : IParser
    {
        private string _baseIkeaUri = "http://www.ikea.com";
        private string _baseIkeaCatalogUri = "http://www.ikea.com/ru/ru/catalog/allproducts/";

        private Dictionary<string, Department> _departments = new Dictionary<string, Department>();
        private Dictionary<string, SubCategory> _subcategories = new Dictionary<string, SubCategory>();
        private Dictionary<string, Product> _products = new Dictionary<string, Product>();

        public List<Department> Departments
        {
            get
            {
                return _departments.Values.ToList();
            }
        }

        public List<Product> Products
        {
            get
            {
                return _products.Values.ToList();
            }
        }

        public List<SubCategory> SubCategories
        {
            get
            {
                return _subcategories.Values.ToList();
            }
        }
        
        public void Parse()
        {
            Parse(_baseIkeaCatalogUri, null);
        }

        public Task ParseAsync(IProgress<ParseProgress> progress = null)
        {
            return Task.Run(() => Parse(_baseIkeaCatalogUri, progress));
        }

        private void Parse(string rootCatalogAddress, IProgress<ParseProgress> progress)
        {
            ParseProgress done = new ParseProgress();

            string rootPage = LoadPage(rootCatalogAddress);
            string pattern = "href\\s*=\\s*[\"'](?<SubUri>\\/ru\\/ru\\/catalog\\/categories\\/departments\\/(?<DepId>\\w+)\\/(?<SubId>\\d+)\\/)";
            foreach (Match match in Regex.Matches(rootPage, pattern, RegexOptions.Compiled))
            {
                string departmentId = match.Groups["DepId"].Value;
                string subcategoryId = match.Groups["SubId"].Value;
                if (!_subcategories.ContainsKey(subcategoryId))
                {
                    string subcategoryPage = LoadPage(_baseIkeaUri + match.Groups["SubUri"].Value);
                    if (subcategoryPage != null)
                    {
                        pattern = "<meta name=\"IRWStats\\.categoryLocal\" content=\"(.*)\"";
                        string departmentName = Regex.Match(subcategoryPage, pattern, RegexOptions.Compiled).Groups[1].Value;

                        pattern = "<meta name=\"IRWStats\\.subCategoryLocal\" content=\"(.*)\"";
                        string subcategoryName = Regex.Match(subcategoryPage, pattern, RegexOptions.Compiled).Groups[1].Value;

                        Department curDep = null;
                        if (!_departments.ContainsKey(departmentId))
                        {
                            Department newDepartment = new Department
                            {
                                Id = departmentId,
                                Name = departmentName,
                                SubCategories = new List<SubCategory>()
                            };
                            _departments.Add(departmentId, newDepartment);
                            curDep = newDepartment;
                            done.Departments++;
                            progress?.Report(done);
                        }
                        else
                        {
                            curDep = _departments[departmentId];
                        }

                        SubCategory newSubCat = new SubCategory
                        {
                            Id = subcategoryId,
                            Name = subcategoryName,
                            Department = curDep,
                            Products = new List<Product>()
                        };
                        _subcategories.Add(subcategoryId, newSubCat);
                        if (curDep.SubCategories == null)
                            curDep.SubCategories = new List<SubCategory>();
                        curDep.SubCategories.Add(newSubCat);
                        done.SubCategories++;
                        progress?.Report(done);

                        pattern = "href=\"(?<product>\\/ru\\/ru\\/catalog\\/products\\/(?<ProdId>\\d+)\\/)\" class=\"productLink\"";
                        foreach (Match productMatch in Regex.Matches(subcategoryPage, pattern, RegexOptions.Compiled))
                        {
                            string productId = productMatch.Groups["ProdId"].Value;
                            if (!_products.ContainsKey(productId))
                            {
                                Product newProduct = ParseProduct(productId);
                                newProduct.SubCategories.Add(newSubCat);
                                _products.Add(newProduct.Id, newProduct);                                
                                newSubCat.Products.Add(newProduct);
                                done.Products++;
                                progress?.Report(done);
                            }
                            else
                            {
                                Product curProduct = _products[productId];
                                curProduct.SubCategories.Add(newSubCat);
                                newSubCat.Products.Add(curProduct);
                            }
                        }
                    }
                }
            }
        }
        
        private Product ParseProduct(string productId)
        {
            Product newProduct = new Product { Id = productId, SubCategories = new List<SubCategory>() };

            string page = LoadPage(ProductLink(productId));
            if (page != null)
            {
                string regexName = "class=\"productName\">\\s*(?<name>.*)\\s*<\\/div>";
                string regexDescription = "class=\"productType\">s*(?<descr>.*)\\s*<strong>";
                string regexPrice = "<span id=\"price1\" class=\"packagePrice\">\\s*(?<price>.*)";
                string regexImg = "<img id=\"productImg\" src='(?<img>\\S*)'";
                
                newProduct.Name = Regex.Match(page, regexName).Groups["name"].Value.Trim();
                newProduct.ShortDescription = Regex.Match(page, regexDescription).Groups["descr"].Value.Trim();
                newProduct.Price = Regex.Match(page, regexPrice).Groups["price"].Value.Trim();
                newProduct.Image = _baseIkeaUri + Regex.Match(page, regexImg).Groups["img"].Value.Trim();
            }
            else
            {
                newProduct.Name = "404";
                newProduct.ShortDescription = "404";
                newProduct.Price = "-1";
                newProduct.Image = string.Empty;
            }
            return newProduct;
        }

        private string LoadPage(string pageUri)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            try
            {
                return client.DownloadStringTaskAsync(pageUri).Result;
            }
            catch
            {
                return null;
            }
        }
        
        private string ProductLink(string productId)
        {
            return _baseIkeaUri + "/ru/ru/catalog/products/" + productId;
        }
    }
}
