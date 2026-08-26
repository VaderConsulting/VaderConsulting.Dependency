using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class HARelationship
    {
        private Server _Server1 = null;
        private Server _Server2 = null;
        private BusinessApplication _Service = null;
        private bool _Mandatory = true; // Can't see what I would use this for, but iServer has it, so for completeness it is here
        private string _GroupName = "";

        public HARelationship()
        {
        }

        public HARelationship(Server Server1, Server Server2, BusinessApplication Service)
        {
            _Server1 = Server1;
            _Server2 = Server2;
            _Service = Service;
        }

        public HARelationship(Server Server1, Server Server2, BusinessApplication Service, bool Mandatory)
        {
            _Server1 = Server1;
            _Server2 = Server2;
            _Service = Service;
            _Mandatory = Mandatory;
        }

        public Server Server1
        {
            get
            {
                return _Server1;
            }
            set
            {
                _Server1 = value;
            }
        }

        public Server Server2
        {
            get
            {
                return _Server2;
            }
            set
            {
                _Server2 = value;
            }
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

        public bool Mandatory
        {
            get
            {
                return _Mandatory;
            }
            set
            {
                _Mandatory = value;
            }
        }

        public string GroupName
        {
            get
            {
                return _GroupName;
            }

            set
            {
                _GroupName = value;
            }
        }

        public override string ToString()
        {
            if (_Mandatory)
            {
                return "Mandatory HA Relationship between " + _Server1.Name + " and " + _Server2.Name + " for " + Service.Name;
            }
            else
            {
                return "Optional HA Relationship between " + _Server1.Name + " and " + _Server2.Name + " for " + Service.Name;
            }
        }
    }
}
