using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeltWindow.ViewModels;
using BeltWindow.Views;

namespace ParentWindow.ViewModels
{
    public class MainViewModel: Conductor<object>
    {
        public void LoadBeltWindow()
        {
            ActivateItemAsync(new BeltViewModel());
        }
    }
}
