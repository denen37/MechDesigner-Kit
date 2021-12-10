using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentWindow.ViewModels
{
    public class DPGraphViewModel: Screen
    {

        public void Ok()
        {
            this.TryCloseAsync(false);
        }
    }
}
