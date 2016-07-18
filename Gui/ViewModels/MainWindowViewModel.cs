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
            CurrentViewModel = new IkeaBrowserViewModel(new DataAccess.IkeaRepository(), new IkeaParser.Parser());
        }

        public object CurrentViewModel { get; set; }
    }

}
