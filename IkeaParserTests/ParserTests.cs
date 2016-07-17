using Microsoft.VisualStudio.TestTools.UnitTesting;
using IkeaParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IkeaParser.Tests
{
    [TestClass()]
    public class ParserTests
    {

        [TestMethod()]
        public void ParseTest()
        {
            Uri ikeaCatalogRoot = new Uri("http://www.ikea.com/ru/ru/catalog/allproducts/");
            IkeaParser.Parser parser = new Parser();
            
            
            
        }
    }
}
