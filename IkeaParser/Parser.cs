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
    public class Parser
    {
        private string _baseIkeaUri = "http://www.ikea.com";

        private Dictionary<string, Department> _departments = new Dictionary<string, Department>();
        private Dictionary<string, SubCategory> _subcategories = new Dictionary<string, SubCategory>();
        private Dictionary<string, Product> _products = new Dictionary<string, Product>();

        public List<Department> Departments {
            get
            {
                return _departments.Values.ToList();
            }
        }

        public List<SubCategory> Subcategories
        {
            get
            {
                return _subcategories.Values.ToList();
            }
        }

        public List<Product> Products
        {
            get
            {
                return _products.Values.ToList();
            }
        }

        public void Parse()
        {
            Parse(_baseIkeaUri + "/ru/ru/catalog/allproducts/");
        }

        public Task ParseAsync()
        {
            return Task.Run(() => Parse());
        }

        public void Parse (string rootCatalogAddress)
        {
            string rootPage = LoadPage(rootCatalogAddress);
            string pattern = "href\\s*=\\s*[\"'](\\/ru\\/ru\\/catalog\\/categories\\/departments\\/(\\w+)\\/(\\d+)\\/)";
            foreach (Match match in Regex.Matches(rootPage, pattern, RegexOptions.Compiled)) 
            {
                string depId = match.Groups[2].Value;
                string subcategoryId = match.Groups[3].Value;
                if (!_subcategories.ContainsKey(subcategoryId))
                {
                    string subcategoryPage = LoadPage(_baseIkeaUri + match.Groups[1].Value);
                    if (subcategoryPage != null)
                    {
                        pattern = "<meta name=\"IRWStats\\.categoryLocal\" content=\"(.*)\"";
                        string departmentName = Regex.Match(subcategoryPage, pattern, RegexOptions.Compiled).Groups[1].Value;

                        pattern = "<meta name=\"IRWStats\\.subCategoryLocal\" content=\"(.*)\"";
                        string subcategoryName = Regex.Match(subcategoryPage, pattern, RegexOptions.Compiled).Groups[1].Value;

                        Department curDep = null;
                        if (!_departments.ContainsKey(depId))
                        {
                            Department newDep = new Department { Id = depId, Name = departmentName };
                            _departments.Add(depId, newDep);
                            curDep = newDep;
                        }
                        else
                        {
                            curDep = _departments[depId];
                        }
                        SubCategory newSubCat = new SubCategory { Id = subcategoryId, Name = subcategoryName, Department = curDep };
                        _subcategories.Add(subcategoryId, newSubCat);
                        curDep.SubCategories.Add(newSubCat);

                        pattern = "href=\"(?<product>\\/ru\\/ru\\/catalog\\/products\\/(?<ProdId>\\d+)\\/)\" class=\"productLink\"";
                        foreach (Match productMatch in Regex.Matches(subcategoryPage, pattern, RegexOptions.Compiled))
                        {
                            string productId = productMatch.Groups["ProdId"].Value;
                            if (!_products.ContainsKey(productId))
                            {
                                string productLink = productMatch.Groups["product"].Value;
                                string productPage = LoadPage(_baseIkeaUri + productLink);
                                if (productPage != null)
                                {
                                    string regexName = "class=\"productName\">\\s*(?<pname>.*)\\s*<\\/div>";
                                    string regexType = "class=\"productType\">s*(?<ptype>.*)\\s*<strong>";
                                    string regexPrice = "<span id=\"price1\" class=\"packagePrice\">\\s*(?<price>.*)";
                                    string regexImg = "<img id=\"productImg\" src='(?<src>\\S*)'";

                                    Product newProduct = new Product();
                                    newProduct.Id = productId;
                                    newProduct.Name = Regex.Match(productPage, regexName).Groups["pname"].Value.Trim();
                                    newProduct.ShortDescription = Regex.Match(productPage, regexType).Groups["ptype"].Value.Trim();
                                    newProduct.Price = Regex.Match(productPage, regexPrice).Groups["price"].Value.Trim();
                                    newProduct.Image = _baseIkeaUri + Regex.Match(productPage, regexImg).Groups["src"].Value.Trim();
                                    newProduct.SubCategories.Add(newSubCat);
                                    _products.Add(newProduct.Id, newProduct);
                                    newSubCat.Products.Add(newProduct);
                                }
                                else
                                {
                                    Product newProduct = new Product { Id = productId, Name = "404" };
                                    newProduct.SubCategories.Add(newSubCat);
                                    _products.Add(newProduct.Id, newProduct);
                                }
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
        
        private string LoadPage(string pageUri)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            try
            {
                string page = client.DownloadStringTaskAsync(pageUri).Result;

                return page;
            }
            catch
            {
                return null;
            }
        }

                

    }
}
