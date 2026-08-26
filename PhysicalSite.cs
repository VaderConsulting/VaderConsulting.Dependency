using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class PhysicalSite
    {
        public enum PhysicalSiteType : int
        {
            NotProtected = 0,
            Protected = 1,
            Recovery = 2
        }

        private string _Name = "";
        private PhysicalSiteType _PhysicalSiteType = PhysicalSiteType.NotProtected;
        private bool _Internal = true;
        private string _vSphereConnectionString = "";
        private List<Server> _Servers = new List<Server>();

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

        public PhysicalSiteType SiteType
        {
            get
            {
                return _PhysicalSiteType;
            }

            set
            {
                _PhysicalSiteType = value;
            }
        }

        public bool Internal
        {
            get
            {
                return _Internal;
            }

            set
            {
                _Internal = value;
            }
        }

        public string vSphereConnectionString
        {
            get
            {
                return _vSphereConnectionString;
            }

            set
            {
                _vSphereConnectionString = value;
            }
        }

        public PhysicalSite(string Name)
        {
            _Name = Name;
        }

        public List<Server> Servers
        {
            get
            {
                return _Servers;
            }
        }

        public void AddServer(Server Server)
        {
            Server.PhysicalSite = this;
            _Servers.Add(Server);
        }

        public override string ToString()
        {
            return _Name;
        }

        
    }
}
