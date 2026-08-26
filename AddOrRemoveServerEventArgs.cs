using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class AddOrRemoveServerEventArgs : EventArgs
    {
        private Server _Server = null;

        public AddOrRemoveServerEventArgs()
        {

        }

        public AddOrRemoveServerEventArgs(Server Server)
        {
            _Server = Server;
        }

        public Server Server
        {
            get
            {
                return _Server;
            }
            set
            {
                _Server = value;
            }
        }

    }
}
