using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GearWindow.Models;
using System.Data;
using Dapper;
using System.Windows;

namespace GearWindow.ViewModels
{
    class GearAppViewModel: Screen
    {
        private List<Applications> apps = new List<Applications>();
        private BindableCollection<Applications> _UIapps;
        private IEventAggregator _events;
        private Applications _app;

        public GearAppViewModel(IEventAggregator events)
        {
            _events = events;
            GetApplications();
        }

        public string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        private void GetApplications()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("SpurGearDrive")))
            {
                try
                {
                    apps = connection.Query<Applications>("SELECT * FROM dbo.RecommendedDesignLife").ToList();
                }
                catch (SystemException e)
                {

                    MessageBox.Show(e.Message, "Database Connection Failed");
                }
            }

            _UIapps = new BindableCollection<Applications>(apps);
        }

        public BindableCollection<Applications> GearApps 
        { 
            get { return _UIapps; }
            private set { _UIapps = value; }
        }

        public Applications SelectedRow 
        { 
            get { return _app; }
            set 
            { 
                _app = value;
                NotifyOfPropertyChange(() => SelectedRow);
                NotifyOfPropertyChange(() => CanSelect);
            }
        }

        public bool CanSelect
        {
            get
            {
                bool canSelect = false;

                if (SelectedRow != null)
                {
                    canSelect = true;
                }

                return canSelect;
            }
        }

        public void Select()
        {
            _events.PublishOnUIThreadAsync(SelectedRow);
        }

        public void Cancel()
        {
            this.TryCloseAsync(false);
        }
    }
}
