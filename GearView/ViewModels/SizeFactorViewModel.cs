﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearWindow.ViewModels
{
    public class SizeFactorViewModel: Screen
    {
        public void Cancel()
        {
            this.TryCloseAsync(false);
        }
    }
}
