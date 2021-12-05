using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BeltWindow.EventsModel
{
    public class MsgBoxParam
    {
        private string _msg;
        private string _cap;
        private MessageBoxResult msgResult;
        private MessageBoxOptions msgOptions;
        private MessageBoxButton msgButton;

        public MsgBoxParam(string msg, string cap = "Belt Drive")
        {
            _msg = msg;
            _cap = cap;
        }

        public string Message
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public string Caption
        {
            get { return _cap; }
            set { _cap = value; }
        }

        public MessageBoxOptions MsgOptions
        {
            get { return msgOptions; }
            set { msgOptions = value; }
        }

        public MessageBoxResult MsgResult
        {
            get { return msgResult; }
            set { msgResult = value; }
        }

        public MessageBoxButton MsgButton
        {
            get { return msgButton; }
            set { msgButton = value; }
        }

    }
}
