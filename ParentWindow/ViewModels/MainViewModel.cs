using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParentWindow.Views;


namespace ParentWindow.ViewModels
{
    public class MainViewModel: Conductor<object>
    {
        private SimpleContainer _container;
        private IEventAggregator _event;
        private IWindowManager _manager;
        public MainViewModel(SimpleContainer container, IEventAggregator @event, IWindowManager manager)
        {
            _event = @event;
            _container = container;
            _manager = manager;
        }
        public void LoadBeltWindow()
        {
            ActivateItemAsync(new BeltViewModel());
        }

        public void LoadGearWindow()
        {
            GearViewModel view = _container.GetInstance<GearViewModel>();
            ActivateItemAsync(view);
            _manager.ShowWindowAsync(view);
        }
    }
}
