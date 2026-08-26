using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VaderConsulting.Dependency
{
    public class Service : ComponentBase
    {
        #region Fields

        private List<Server> _ServersUsedBy = new List<Server>();
        private List<Relationship> _Relationships = new List<Relationship>();
        private List<Server> _DependantServers = new List<Server>();
        private int _Version = 0;
        private string _DisplayName = "";
        private string _Description = "";

        #endregion

        #region Constructors

        public Service(string Name)
            : base()
        {
            base.Name = Name;
        }

        public Service(string Name, Global.HealthState State)
            : base()
        {
            base.Name = Name;
            //base.State = State;
            base.SetSystemHealthState(State, "");
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// These are the Servers that require this Service (first-level only)
        /// </summary>
        public List<Server> ServersUsedBy
        {
            get
            {
                return _ServersUsedBy;
            }
        }

        public List<Relationship> Relationships
        {
            get
            {
                return _Relationships;
            }
        }

        public List<Server> DependantServers
        {
            get
            {
                return _DependantServers;
            }
            set
            {
                _DependantServers = value;
            }
        }

        public int Version
        {
            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }
        
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        #endregion

        #region Private Methods

        //internal void AddProvidingServer(string Server)
        //{
        //    AddProvidingServer(new Server(Server));
        //}

        //internal void AddComponentService(string Service)
        //{
        //    AddComponentService(new Service(Service));
        //}

        //internal void RemoveProvidingServer(string Server)
        //{
        //    RemoveProvidingServer(new Server(Server));
        //}

        //internal void RemoveComponentService(string Service)
        //{
        //    RemoveComponentService(new Service(Service));
        //}

        //private Service GetExistingService(string ServiceName)
        //{
        //    return GetExistingService(new Service(ServiceName));
        //}

        //private Service GetExistingService(Service Service)
        //{
        //    return _ComponentServices.Find(x => x.Name == Service.Name);
        //}

        //private Server GetExistingServer(string ServerName)
        //{
        //    return GetExistingServer(new Server(ServerName));
        //}

        //private Server GetExistingServer(Server Server)
        //{
        //    return _ComponentServers.Find(x => x.Name == Server.Name);
        //}

        #endregion

        #region Internal Methods

        //private void AddProvidingServer(Server Server)
        //{
        //    if (!Server.ServicesProvided.Exists(x => x.Name == this.Name))
        //    {
        //        Server.AddProvidedService(this);
        //    }

        //    if (!_ComponentServers.Exists(x => x.Name == Server.Name))
        //    {
        //        _ComponentServers.Add(Server);
        //    }
        //}

        //private void AddComponentService(Service Service)
        //{
        //    //if (!_Services.Exists(x => x.Name == Service.Name))
        //    //{
        //    //    _Services.Add(Service);
        //    //}

        //    if (!_ComponentServices.Exists(x => x.Name == this.Name))
        //    {
        //        _ComponentServices.Add(Service);
        //    }
        //}

        //private void RemoveProvidingServer(Server Server)
        //{
        //    Server ServerToRemove = _ComponentServers.Find(x => x.Name == Server.Name);

        //    if (ServerToRemove != null)
        //    {
        //        _ComponentServers.Remove(ServerToRemove);
        //    }

        //    foreach (Service Service in _ComponentServices)
        //    {
        //        Server ComponentServer = Service._ComponentServers.Find(x => x.Name == Service.Name);

        //        if (ComponentServer != null)
        //        {
        //            Service.ComponentServers.Remove(ComponentServer);
        //        }
        //    }
        //}

        //private void RemoveComponentService(Service Service)
        //{
        //    Service ServiceToRemove = _ComponentServices.Find(x => x.Name == Service.Name);

        //    if (ServiceToRemove != null)
        //    {
        //        _ComponentServices.Remove(ServiceToRemove);
        //    }

        //    foreach (Server Server in _ComponentServers)
        //    {
        //        Service ProvidedService = Server.ServicesProvided.Find(x => x.Name == Service.Name);

        //        if (ProvidedService != null)
        //        {
        //            Server.RemoveProvidedService(ProvidedService);
        //        }
        //    }
        //}



        #endregion

        #region Public Methods

        //public void AddDependency(Relationship Relationship)
        //{
        //    _Relationships.Add(Relationship);

        //    if (Relationship.ConnectedComponentType == Dependency.Relationship.ComponentType.Server)
        //    {
        //        if (Global.GetExistingServer(Relationship.ComponentServer, _ComponentServers) != null)
        //        {
        //            _ComponentServers.Add(Relationship.ComponentServer);
        //        }
        //    }
        //    else
        //    {
        //        if (Global.GetExistingService(Relationship.ComponentService, _ComponentServices) != null)
        //        {
        //            _ComponentServices.Add(Relationship.ComponentService);
        //            _ServicesUsedBy.Add(Relationship.Service);
        //        }
        //    }
        //}

        //public void RemoveDependency(Relationship Relationship)
        //{
        //    _Relationships.Remove(Relationship);

        //    if (Relationship.ConnectedComponentType == Dependency.Relationship.ComponentType.Server)
        //    {
        //        if (Global.GetExistingServer(Relationship.ComponentServer, _ComponentServers) != null)
        //        {
        //            _ComponentServers.Remove(Relationship.ComponentServer);
        //        }
        //    }
        //    else
        //    {
        //        if (Global.GetExistingService(Relationship.ComponentService, _ComponentServices) != null)
        //        {
        //            _ComponentServices.Remove(Relationship.ComponentService);
        //            _ServicesUsedBy.Remove(Relationship.Service);
        //        }
        //    }
        //}

        //internal Relationship AddComponentServer(string ServerName)
        //{
        //    Server Server = new Server(ServerName);

        //    return AddComponentServer(ref Server);
        //}

        //public Relationship AddComponentServer(ref Server Server)
        //{
        //    // Add Server as a ComponentServer of this Service, returning a Relationship describing the result
        //    Relationship Relationship = new Relationship();

        //    this.ComponentServers.Add(Server);
        //    Relationship.Service = this;
        //    Relationship.ComponentServer = Server;

        //    // Look for this Server in my list of ComponentServers
        //    if (Global.GetExistingService(this, _ComponentServices) == null)
        //    //if (!Server.ProvidedServices.Contains(this))
        //    {               
        //        Server.AddRelationship(Relationship, Relationships);
        //    }
        //    else
        //    {
        //        // Relationship already exists between this Service and the Server
        //        int i = 1;
        //        Server.ProvidedServices.Add(this);
        //    }

        //    this.Relationships.Add(Relationship);

        //    return Relationship;
        //}

        //internal Relationship AddComponentService(string ServiceName)
        //{
        //    return AddComponentService(new Service(ServiceName));
        //}

        //public Relationship AddComponentService(Service Service)
        //{
        //    Relationship Relationship = new Relationship();

        //    this.ComponentServices.Add(Service);
        //    Relationship.Service = this;
        //    Relationship.ComponentService = Service;

        //    //if (!Service.ComponentServices.Contains(this))
        //    //{
        //    //    Service.ComponentServices.Add(this);
        //    //}

        //    this.Relationships.Add(Relationship);

        //    return Relationship;
        //}

        #endregion

        #region Overrides

        

        #endregion

    }
}
