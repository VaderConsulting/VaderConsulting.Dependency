using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class BusinessService : ComponentBase
    {

        #region Fields

        private TimeSpan _CalculatedRecoveryTime = new TimeSpan(21, 0, 0, 0);
        private Global.RecoveryStream _CalculatedStream = Global.RecoveryStream.None3;
        private List<BusinessService> _ComponentBusinessServices = new List<BusinessService>();
        private List<Server> _ComponentServers = new List<Server>();
        private List<BusinessService> _DependentBusinessServices = new List<BusinessService>();
        private List<Server> _DependentServers = new List<Server>();
        private string _Description = "";
        private string _DisplayName = "";
        private Drawing _Drawing = null;
        private List<HARelationship> _HARelationships = new List<HARelationship>();
        private List<Server> _HAServers = new List<Server>();
        private bool _InScope = false;
        private string _ManagementPackXML = "";
        private bool _OverrideInScope = false;
        private List<RecoveryTask> _RecoveryTasks = new List<RecoveryTask>();
        private List<Relationship> _Relationships = new List<Relationship>();
        private bool _ShowInServiceCatalogue = true;
        private VaderConsulting.Database.SQLServer _SQL = null;
        private int _RunbookOrder = int.MaxValue;
        private bool _StateSetAtServiceLevel = false;
        private bool _SubService = false;
        private Global.RecoveryTier _Tier = Global.RecoveryTier.Three;
        private int _Version = 0;
        private bool _HighlyAvailable = false;
        private string _Comment1 = "";
        private string _Comment2 = "";
        private string _Comment3 = "";
        private string _Comment4 = "";
        private string _Comment5 = "";

        #endregion

        #region Properties

        public TimeSpan CalculatedRecoveryTime
        {
            get
            {
                return _CalculatedRecoveryTime;
            }
            set
            {
                _CalculatedRecoveryTime = value;
            }
        }

        public Global.RecoveryStream CalculatedStream
        {
            get
            {
                return _CalculatedStream;
            }
            set
            {
                _CalculatedStream = value;
            }
        }

        /// <summary>
        /// These are the Services that this Service requires (first-level only)
        /// </summary>
        public List<BusinessService> ComponentBusinessServices
        {
            get
            {
                return _ComponentBusinessServices;
            }
        }

        /// <summary>
        /// These are the Servers that this Service requires (first-level only)
        /// </summary>
        public List<Server> ComponentServers
        {
            get
            {
                return _ComponentServers;
            }
        }

        public List<BusinessService> DependentBusinessServices
        {
            get
            {
                return _DependentBusinessServices;
            }
            set
            {
                _DependentBusinessServices = value;
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

        public Drawing Drawing
        {
            get
            {
                return _Drawing;
            }
            set
            {
                _Drawing = value;
            }
        }

        public List<HARelationship> HARelationships
        {
            get
            {
                return _HARelationships;
            }
        }

        public List<Server> HAServers
        {
            get
            {
                return _HAServers;
            }
            set
            {
                _HAServers = value;
            }
        }

        public bool HighlyAvailable
        {
            get
            {
                return _HighlyAvailable;
            }
            set
            {
                _HighlyAvailable = value;
            }
        }

        //public System.Drawing.Image Image
        //{
        //    get
        //    {
        //        return _Image;
        //    }
        //    set
        //    {
        //        _Image = value;
        //    }
        //}

        public bool InScope
        {
            get
            {
                return _InScope;
            }
            set
            {
                _InScope = value;
            }
        }

        public string ManagementPackXML
        {
            get
            {
                return _ManagementPackXML;
            }
            set
            {
                _ManagementPackXML = value;
            }
        }

        public List<BusinessService> MandatoryBusinessServices
        {
            get
            {
                List<BusinessService> Result = new List<BusinessService>();

                foreach (BusinessService Service in _ComponentBusinessServices)
                {
                    Relationship Relationship = Global.GetRelationship(this, Service, _Relationships);

                    if (Relationship != null)
                    {
                        if (Relationship.SystemMandatory && !Relationship.StreamDependency)
                        {
                            if (!Result.Contains(Service))
                            {
                                Result.Add(Service);
                            }
                        }
                    }
                    //if (Global.GetRelationship(this, Service, _Relationships).SystemMandatory)
                    //{
                    //    if (!Result.Contains(Service))
                    //    {
                    //        Result.Add(Service);
                    //    }
                    //}
                }

                return Result;
            }
        }

        public List<Server> MandatoryServers
        {
            get
            {
                List<Server> Result = new List<Server>();

                foreach (Server Server in _ComponentServers)
                {
                    Relationship Relationship = Global.GetRelationship(this, Server, _Relationships);

                    if (Relationship != null)
                    {
                        if (Relationship.SystemMandatory && !Relationship.StreamDependency)
                        {
                            if (!Result.Contains(Server))
                            {
                                Result.Add(Server);
                            }
                        }
                    }
                }

                return Result;
            }
        }

        //public float CalculatedOrder
        //{
        //    get
        //    {
        //        float Order = 0.0F;
        //        float.TryParse(this.SRMRecoveryIndex.ToString() + "." + this.SRMPriorityGroupIndex.ToString(), out Order);

        //        return Order;
        //    }
        //}

        public bool OverrideInScope
        {
            get
            {
                return _OverrideInScope;
            }
            set
            {
                _OverrideInScope = value;
            }
        }

        public List<RecoveryTask> RecoveryTasks
        {
            get
            {
                return _RecoveryTasks;
            }
            set
            {
                _RecoveryTasks = value;
            }
        }

        public List<Relationship> Relationships
        {
            get
            {
                return _Relationships;
            }
        }

        public bool ShowInServiceCatalogue
        {
            get
            {
                return _ShowInServiceCatalogue;
            }
            set
            {
                _ShowInServiceCatalogue = value;
            }
        }

        public VaderConsulting.Database.SQLServer SQL
        {
            get
            {
                return _SQL;
            }
            set
            {
                _SQL = value;
            }
        }

        public int RunbookOrder
        {
            get
            {
                return _RunbookOrder;
            }
            set
            {
                _RunbookOrder = value;
            }
        }

        public bool StateSetAtServiceLevel
        {
            get
            {
                return _StateSetAtServiceLevel;
            }
            set
            {
                _StateSetAtServiceLevel = value;
            }
        }

        public bool SubService
        {
            get
            {
                return _SubService;
            }
            set
            {
                _SubService = value;
            }
        }

        ///// <summary>
        ///// These are the Services that require this Service (first-level only)
        ///// </summary>
        //public List<BusinessService> BusinessServicesUsedBy
        //{
        //    get
        //    {
        //        return _BusinessServicesUsedBy;
        //    }
        //}

        public Global.RecoveryTier Tier
        {
            get
            {
                return _Tier;
            }
            set
            {
                _Tier = value;
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

        public string Comment1
        {
            get
            {
                return _Comment1;
            }

            set
            {
                _Comment1 = value;
            }
        }

        public string Comment2
        {
            get
            {
                return _Comment2;
            }

            set
            {
                _Comment2 = value;
            }
        }

        public string Comment3
        {
            get
            {
                return _Comment3;
            }

            set
            {
                _Comment3 = value;
            }
        }

        public string Comment4
        {
            get
            {
                return _Comment4;
            }

            set
            {
                _Comment4 = value;
            }
        }

        public string Comment5
        {
            get
            {
                return _Comment5;
            }

            set
            {
                _Comment5 = value;
            }
        }

        #endregion

        #region Constructors

        public BusinessService(string Name)
            : base()
        {
            base.Name = Name;
        }

        public BusinessService(string Name, Global.HealthState State)
            : base()
        {
            base.Name = Name;
            //base.State = State;
            base.SetSystemHealthState(State, "");
        }

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

        #region Public Methods

        public void AddHARelationship(HARelationship Relationship)
        {
            if (!_HAServers.Contains(Relationship.Server1))
            {
                _HAServers.Add(Relationship.Server1);
            }

            if (!_HAServers.Contains(Relationship.Server2))
            {
                _HAServers.Add(Relationship.Server2);
            }

            _HARelationships.Add(Relationship);
        }

        public void AddRelationship(Relationship Relationship)
        {
            // Check if the relationship exists
            if (Global.GetExistingRelationship(Relationship, _Relationships) == null)
            {
                switch (Relationship.ToType)
                {
                    case Dependency.Relationship.ComponentType.BusinessService:

                        // Check if the Service exists
                        if (Global.GetExistingBusinessService(Relationship.ToBusinessService, _ComponentBusinessServices) == null)
                        {
                            // Add the Component Service
                            _ComponentBusinessServices.Add(Relationship.ToBusinessService);
                        }

                        // Add the relationship
                        _Relationships.Add(Relationship);

                        break;
                    case Dependency.Relationship.ComponentType.Server:

                        // Check if the Service exists
                        if (Global.GetExistingServer(Relationship.ToServer, _ComponentServers) == null)
                        {
                            // Add the Component Service
                            _ComponentServers.Add(Relationship.ToServer);
                        }

                        // Add the relationship
                        _Relationships.Add(Relationship);

                        break;
                }
            }
        }

        public void CalculateState(string Reason)
        {
            // If the state hasn't been set explicitly at the Service Level (it should be a combination of the states of it's Components),
            // then work out it's state.
            if (!_StateSetAtServiceLevel) // <- to allow emulation or to ignore component health state
            {
                #region No Components

                if (this.MandatoryBusinessServices.Count == 0 && this.MandatoryServers.Count == 0)      // No components
                {
                    this.SetSystemHealthState(Global.HealthState.OK, "This Business Service has no Components");
                }

                #endregion

                #region Services only

                else if (this.MandatoryServers.Count == 0 && this.MandatoryBusinessServices.Count > 0)  // Has Services only
                {
                    // System State
                    BusinessService ComponentService = this.MandatoryBusinessServices.OrderByDescending(s => s.SystemState)
                                                                                     .FirstOrDefault();

                    if (ComponentService != null)
                    {
                        this.SetSystemHealthState(ComponentService.SystemState, ComponentService + " Health State is " + ComponentService.SystemState);
                    }

                    // User State
                    ComponentService = this.MandatoryBusinessServices.OrderByDescending(s => s.UserState)
                                                                     .FirstOrDefault();

                    if (ComponentService != null)
                    {
                        this.SetUserHealthState(ComponentService.UserState, ComponentService + " Health State is " + ComponentService.UserState);
                    }
                }

                #endregion

                #region Servers only

                else if (this.MandatoryBusinessServices.Count == 0 && this.MandatoryServers.Count > 0) // Has Servers only
                {
                    // System State
                    Server ComponentServer = this.MandatoryServers.OrderByDescending(s => s.SystemState)
                                                                  .FirstOrDefault();

                    if (ComponentServer != null)
                    {
                        this.SetSystemHealthState(ComponentServer.SystemState, ComponentServer + " Health State is " + ComponentServer.SystemState);
                    }

                    // User State
                    ComponentServer = this.MandatoryServers.OrderByDescending(s => s.UserState)
                                                           .FirstOrDefault();

                    if (ComponentServer != null)
                    {
                        this.SetUserHealthState(ComponentServer.UserState, ComponentServer + " Health State is " + ComponentServer.UserState);
                    }
                }

                #endregion

                #region Services and Servers

                else if (this.MandatoryBusinessServices.Count > 0 && this.MandatoryServers.Count > 0)   // Has Servers and Services
                {
                    #region Service

                    // Service System State
                    BusinessService WorstSystemStateService = _ComponentBusinessServices.OrderByDescending(s => s.SystemState)
                                                                                  .FirstOrDefault();

                    // Service User State
                    BusinessService WorstUserStateService = this.MandatoryBusinessServices.OrderByDescending(s => s.UserState)
                                                                                      .FirstOrDefault();

                    #endregion

                    #region Server

                    // System State
                    Server WorstSystemStateServer = this.MandatoryServers.OrderByDescending(s => s.SystemState)
                                                                         .FirstOrDefault();

                    // User State
                    Server WorstUserStateServer = this.MandatoryServers.OrderByDescending(s => s.UserState)
                                                                       .FirstOrDefault();

                    #endregion

                    // Now work out the worst of the two User states (Service or Server)
                    Global.HealthState WorstUserState = WorstUserStateServer.UserState;

                    if (WorstUserStateService.UserState > WorstUserState)
                    {
                        WorstUserState = WorstUserStateService.UserState;
                        Reason = WorstUserStateService.Name + " Health State is " + WorstUserStateService.UserState;
                    }

                    // Now work out the worst of the two System states (Service or Server)
                    Global.HealthState WorstSystemState = WorstSystemStateServer.SystemState;

                    if (WorstSystemStateService.SystemState > WorstSystemState)
                    {
                        WorstSystemState = WorstSystemStateService.SystemState;
                        Reason = WorstSystemStateService.Name + " Health State is " + WorstSystemStateService.UserState;
                    }

                    // Finally, set the values
                    this.SetSystemHealthState(WorstSystemState, Reason);
                    this.SetUserHealthState(WorstUserState, Reason);

                }

                #endregion            
            }
        }

        public Relationship GetRelationship(BusinessService Service)
        {
            return _Relationships.Find(x => Service == x.ToBusinessService);
        }

        public HARelationship GetHARelationship(HARelationship Relationship)
        {
            return _HARelationships.Find(x => Relationship == x);
        }

        public Relationship GetRelationship(Server Server)
        {
            return _Relationships.Find(x => Server == x.ToServer);
        }

        public void Save(VaderConsulting.Database.SQLServer SQL)
        {
            _SQL = SQL;
            Save();
        }

        public void Save()
        {
            string Query = "";
            string SingleQuote = "'";
            string TwoSingleQuotes = "''";

            // TODO:  Get attribute ID's and bring them back to configurable options

            // Runbook Order
            Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + _RunbookOrder.ToString() + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'E94D58B7-8942-4A05-92EA-CD5942C49C07' AND VersionId = '" + base.VersionID.ToString() + "'";
            _SQL.ExecuteNonQuery(Query);

            // Service Display Name
            Query = "UPDATE AttributeValueText SET AttributeValue = '" + _DisplayName.Replace(SingleQuote, TwoSingleQuotes) + "' WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = '1AC31119-F69D-440A-88BF-F177D760603F' AND VersionId = '" + base.VersionID.ToString() + "'";
            _SQL.ExecuteNonQuery(Query);

            // Show in Service Catalogue
            Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + Convert.ToInt32(_ShowInServiceCatalogue) + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'F80FD169-6EC2-47E9-951F-4D8988D6C166' AND VersionId = '" + base.VersionID.ToString() + "'";
            _SQL.ExecuteNonQuery(Query);

            // Sub Service
            Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + Convert.ToInt32(_SubService) + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'FA0F55D9-EE64-4159-88D4-2290C1281979' AND VersionId = '" + base.VersionID.ToString() + "'";
            _SQL.ExecuteNonQuery(Query);

            // In Scope
            Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + Convert.ToInt32(_InScope) + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'D29C2600-F53E-4B25-8F4A-818FC7656FAE' AND VersionId = '" + base.VersionID.ToString() + "'";
            _SQL.ExecuteNonQuery(Query);

            // Service Comment 1
            Query = "UPDATE AttributeValueText SET AttributeValue = '" + _Comment1.Replace(SingleQuote, TwoSingleQuotes) + "' WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'D61B7818-63E2-4533-8940-86C206181175' AND VersionId = '" + base.VersionID.ToString() + "'";
            _SQL.ExecuteNonQuery(Query);

            // Save the associated drawing
            if (_Drawing != null)
            {
                _Drawing.Save(_SQL);
            }
        }

        #endregion

        #region Overrides

        public override void RaiseNewSystemState(Global.HealthState NewState, Global.HealthState OldState, string Name, string Text)
        {
            if (NewState != OldState)
            {
                base.RaiseNewSystemState(NewState, OldState, Name, Text);

                CalculateState("");

                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.Print("[INF1120] " + base.Name + " (Business Service) has changed state from " + OldState + " to " + NewState);
                }
            }
        }

        public override string ToString()
        {
            return base.Name + " = Stream " + _CalculatedStream;
        }

        #endregion
    }
}
