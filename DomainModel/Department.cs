﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Department
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual List<SubCategory> SubCategories { get; set; }
    }
}
