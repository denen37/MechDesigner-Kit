using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeltWindow.EventsModel;
using System.Windows;

namespace BeltWindow.Models
{
    public class MessagesModel 
    {
        //private BeltProperties _belt = new BeltProperties();

        //public MessagesModel(BeltProperties belt)
        //{
        //    _belt = belt;
        //}

        public void _belt_message(MsgBoxParam m)
        {
            if (m.MsgButton != default)
            {
                MessageBox.Show(m.Message, m.Caption, m.MsgButton);
                return;
            }
                
            MessageBox.Show(m.Message, m.Caption);
        }


        //public event EventHandler<MessageBoxEventArgs>;

    }
}
