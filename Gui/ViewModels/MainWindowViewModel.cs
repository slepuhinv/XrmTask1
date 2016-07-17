using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            CurrentViewModel = new IkeaBrowserViewModel();
        }

        public object CurrentViewModel { get; set; }
    }

}
