using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class AddOrRemoveBusinessApplicationEventArgs : EventArgs
    {
        private BusinessApplication _Service = null;

        public AddOrRemoveBusinessApplicationEventArgs()
        {

        }

        public AddOrRemoveBusinessApplicationEventArgs(BusinessApplication Service)
        {
            _Service = Service;
        }

        public BusinessApplication Service
        {
            get
            {
                return _Service;
            }
            set
            {
                _Service = value;
            }
        }

    }
}
