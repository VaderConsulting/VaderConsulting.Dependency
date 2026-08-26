using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class HAGroup
    {
        private string _Name = "";
        private List<Server> _Servers = new List<Server>();
        private Global.RecoveryStream _Stream = Global.RecoveryStream.Undefined;

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }

        public List<Server> Servers
        {
            get
            {
                return _Servers;
            }
        }

        public Global.RecoveryStream Stream
        {
            get
            {
                return _Stream;
            }

            set
            {
                _Stream = value;
            }
        }

        /// <summary>
        /// Determine resultant Stream, storing the result as publically accessible Stream
        /// </summary>
        private void CalculateStream()
        {
            //Global.RecoveryStream ServerStream = Global.RecoveryStream.Undefined;
            //Global.RecoveryStream ResultantStream = Global.RecoveryStream.Undefined;
            //bool FoundRecoverySite = false;
            //bool FoundProtectedSite = false;
            //bool FoundNotProtected = false;
            //bool External = false;
            
            //foreach (Server Server in _Servers)
            //{
            //    switch (Server.PhysicalSite.SiteType)
            //    {
            //        case PhysicalSite.PhysicalSiteType.Protected:
            //            FoundProtectedSite = true;
            //            ServerStream = Global.RecoveryStream.None3;
            //            break;
            //        case PhysicalSite.PhysicalSiteType.Recovery:
            //            FoundRecoverySite = true;
            //            ServerStream = Global.RecoveryStream.None2;
            //            break;
            //        case PhysicalSite.PhysicalSiteType.NotProtected:
            //            FoundNotProtected = true;
            //            ServerStream = Global.RecoveryStream.None1;
            //            break;
            //        default:
            //            ServerStream = Global.RecoveryStream.Undefined;
            //            break;
            //    }

            //    if (!Server.PhysicalSite.Internal)
            //    {
            //        ServerStream = Global.RecoveryStream.None1;
            //    }
            //}
        }

        public void AddServer(Server Server)
        {
            if (Server != null)
            {
                _Servers.Add(Server);

                CalculateStream();
            }
        }

        public void RemoveServer(Server Server)
        {
            if (Server != null)
            {
                _Servers.Remove(Server);

                CalculateStream();
            }
        }
    }
}
