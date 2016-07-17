using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkeaParser
{
    public interface IParser
    {
        void Parse(string source);

        

        List<Department> Departments { get; }

    }
}
