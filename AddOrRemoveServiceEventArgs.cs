using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class AddOrRemoveServiceEventArgs : EventArgs
    {
        private Service _Service = null;

        public AddOrRemoveServiceEventArgs()
        {

        }

        public AddOrRemoveServiceEventArgs(Service Service)
        {
            _Service = Service;
        }

        public Service Service
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
