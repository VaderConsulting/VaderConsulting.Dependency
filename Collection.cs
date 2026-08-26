using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class Collection
    {
        #region Fields

        private List<BusinessApplication> _Services = new List<BusinessApplication>();
        private List<Server> _Servers = new List<Server>();
        private Queue<string> _Runbook = null;
        private List<Relationship> _Relationships = new List<Relationship>();
        private TopologicalSort<string> _TopoSort = null;
        private List<PhysicalSite> _Sites = new List<PhysicalSite>();
        private List<String> _Errors = new List<string>();

        #endregion

        #region Properties

        public List<BusinessApplication> Services
        {
            get
            {
                return _Services;
            }

            set
            {
                _Services = value;
            }
        }

        public List<Server> Servers
        {
            get
            {
                return _Servers;
            }
        }

        public List<Relationship> Relationships
        {
            get
            {
                return _Relationships;
            }
        }

        public List<PhysicalSite> Sites
        {
            get
            {
                return _Sites;
            }

            set
            {
                _Sites = value;
            }
        }

        public List<String> Errors
        {
            get
            {
                return _Errors;
            }

            set
            {
                _Errors = value;
            }
        }

        #endregion

        #region Constructors

        public Collection()
        {
            _TopoSort = new TopologicalSort<string>();
        }

        #endregion

        #region Private Methods

        private bool AddService(ref BusinessApplication Service)
        {
            // Look for the Service
            BusinessApplication ExistingService = Global.GetExistingBusinessApplication(Service.Name, _Services);
            if (ExistingService == null)
            {
                // Couldn't find the Service, so add it  (This should never happen)
                _Services.Add(Service);

                ExistingService = Service;
            }
            else
            {

            }

            // Go through each server in the provided Service and ensure the existing Service already has that Server listed
            for (int i = 0; i < ExistingService.ComponentServers.Count; i++)
            {
                Server ComponentServer = ExistingService.ComponentServers[i];

                if (Global.GetExistingServer(ComponentServer, ExistingService.ComponentServers) == null)
                {
                    Relationship Relationship = new Dependency.Relationship(ref ExistingService, ref ComponentServer);

                    this.Relationships.Add(Relationship);
                }

                if (Service.InScope || Service.OverrideInScope)
                {
                    if (!_TopoSort.AddConnection(ExistingService.Name, ComponentServer.Name))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void AddServer(ref Server Server)
        {
            BusinessApplication s = null;
            AddServer(ref Server, ref s);
        }

        private bool AddServer(ref Server Server, ref BusinessApplication ProvidedService)
        {
            if (ProvidedService.InScope)
            {
                if (!_TopoSort.AddConnection(ProvidedService.Name, Server.Name))
                {
                    return false;
                }
            }

            if (Global.GetExistingServer(Server, _Servers) == null)
            {
                // 1. If the server doesn't exist add it
                _Servers.Add(Server);

                // Does the existing Service list already have this Service in it's ServicesProvided list?
                for (Int32 i = 0; i < Server.ProvidedBusinessApplications.Count; i++)
                {
                    BusinessApplication ServerService = Server.ProvidedBusinessApplications[i];

                    if (Global.GetExistingBusinessApplication(ServerService, Server.ProvidedBusinessApplications) == null)
                    {
                        Relationship Relationship = new Dependency.Relationship(ref ServerService, ref Server);

                        this.Relationships.Add(Relationship);

                        if (ServerService.InScope)
                        {
                            if (!_TopoSort.AddConnection(ServerService.Name, Server.Name))
                            {
                                return false;
                            }
                            else
                            {
                                _Services.Add(ServerService);
                            }
                        }
                    }
                }
            }
            else
            {
                // 2. If the server exists, add this service to it's list of provided services
                if (ProvidedService != null)
                {
                    List<Server> ProvidedServiceComponentServers = ProvidedService.ComponentServers;
                    Server ExistingComponentServer = Global.GetExistingServer(Server, ProvidedServiceComponentServers); //ProvidedService.ComponentServers);

                    if (ExistingComponentServer == null)
                    {
                        Relationship Relationship = new Dependency.Relationship(ref ProvidedService, ref Server);

                        this.Relationships.Add(Relationship);

                        if (ProvidedService.InScope)
                        {
                            if (!_TopoSort.AddConnection(ProvidedService.Name, Server.Name))
                            {
                                return false;
                            }
                        }
                    }

                    // Does the existing Server already have this Service in it's ProvidedBusinessApplications list?
                    BusinessApplication ExistingService = Global.GetExistingBusinessApplication(ProvidedService.Name, Server.ProvidedBusinessApplications);
                    if (ExistingService == null)
                    {
                        // Could not find this service, so add it.
                        Server.ProvidedBusinessApplications.Add(ProvidedService);
                    }
                }
            }

            return true;
        }

        private void RemoveServer(Server Server)
        {
            // Remove the Server from the collection
            Server ServerToRemove = _Servers.Find(x => x.Name == Server.Name);

            if (ServerToRemove != null)
            {
                _Servers.Remove(ServerToRemove);
            }

            // Find out if this server provided a service by itself.  If so, also remove that service
            for (Int32 i = 0; i < _Services.Count; i++)
            {
                BusinessApplication Service = _Services[i];

                Server ProvidingServer = Service.ComponentServers.Find(x => x.Name == Server.Name);

                if (ProvidingServer != null)
                {
                    if (Service.ComponentServers.Count == 1)
                    {
                        _Services.Remove(Service);
                    }
                }
            }

            // Remove this server from all Services
            for (Int32 i = 0; i < _Services.Count; i++)
            {
                BusinessApplication Service = _Services[i];

                Server ProvidingServer = Service.ComponentServers.Find(x => x.Name == Server.Name);

                if (ProvidingServer != null)
                {
                    Service.ComponentServers.Remove(ProvidingServer);
                }
            }

            Server.Dispose();
        }

        private void RemoveService(BusinessApplication Service)
        {
            BusinessApplication ServiceToRemove = _Services.Find(x => x.Name == Service.Name);

            if (ServiceToRemove != null)
            {
                // Go through and remove all Servers from this Service
                for (Int32 i = 0; i < ServiceToRemove.ComponentServers.Count; i++)
                {
                    Server Server = ServiceToRemove.ComponentServers[i];

                    // If the providing Server only provided one Service (i.e. this Service), then also remove the Server
                    if (Server.ProvidedBusinessApplications.Count < 2)
                    {
                        RemoveServer(Server);

                        _Servers.Remove(Server);
                    }
                }

                _Services.Remove(ServiceToRemove);

                Service.Dispose();
            }
        }

        private void ClearServices()
        {
            for (int i = _Services.Count - 1; i > -1; i--)
            {
                BusinessApplication Service = _Services[i];

                for (int j = Service.ComponentServers.Count - 1; j > -1; j--)
                {
                    Server Server = _Servers[j];
                    RemoveServer(Server);
                }
                RemoveService(Service);
            }
        }

        private void ClearServers()
        {
            for (int i = _Servers.Count - 1; i > -1; i--)
            {
                Server Server = _Servers[i];
                RemoveServer(Server);
            }
        }

        private List<BusinessApplication> GetUpstreamServiceList(BusinessApplication Service, List<BusinessApplication> ExistingServiceList)
        {
            List<BusinessApplication> UpstreamServices = ExistingServiceList;

            // Firstly determine the things that are reliant upon this service
            foreach (BusinessApplication Dependent in Service.DependentBusinessApplications)
            {
                BuildServiceDependencies(Service, Dependent, ref UpstreamServices);
            }

            // Then find the services that use this new list...
            // Loop through all services...
            foreach (BusinessApplication ServiceFromAllServiceList in _Services)
            {
                BuildServiceDependencies(Service, ServiceFromAllServiceList, ref UpstreamServices);

                #region Delete
                //// For each Service, check it's Component Services
                //foreach (Service ComponentService in ServiceFromAllServiceList.ComponentServices) //ServiceFromAllServiceList.ComponentServices)
                //{
                //    // ... until we find one that relies on this service
                //    if (ComponentService == Service)
                //    {
                //        // Check to ensure we don't already have it in our list
                //        if (Global.GetExistingService(ComponentService, UpstreamServices) == null)
                //        {
                //            // Not in the list, so add it to the List of Upstream Services...
                //            UpstreamServices.Add(ServiceFromAllServiceList);

                //            // check it isn't in our list of Dependent services...
                //            if (Global.GetExistingService(ServiceFromAllServiceList, Service.DependentServices) == null)
                //            {
                //                // .. no it isn't so add it
                //                // (But only if it isn't this service)
                //                if (Service != ServiceFromAllServiceList)
                //                {
                //                    Debug.WriteLine(ServiceFromAllServiceList.Name + " needs " + Service.Name);
                //                    Service.DependentServices.Add(ServiceFromAllServiceList);
                //                    Debug.WriteLine("===========================> Added " + ServiceFromAllServiceList.Name + " to " + Service.Name + " dependencies");

                //                    // Call ourselves recursively to continue building the List
                //                    UpstreamServices = GetUpstreamServiceList(ServiceFromAllServiceList, UpstreamServices);
                //                }
                //            }
                //        }
                //    }
                //}
                //}
                #endregion
            }

            #region Delete
            //// Work out all the Services that require this Service (indirectly)
            //foreach (Service ComponentService in Service.ComponentServices)
            //{
            //    if (ComponentService.Name != Service.Name)
            //    {
            //        //Debug.WriteLine(Service.Name + " needs " + ComponentService.Name);
            //        UpstreamServices.Add(ComponentService);

            //        foreach (Service UpstreamService in ComponentService.ComponentServices)
            //        {
            //            if (UpstreamService.Name != ComponentService.Name)
            //            {
            //                if (Global.GetExistingService(UpstreamService, UpstreamServices) == null)
            //                {
            //                    Debug.WriteLine("Upstream service count for " + Service.Name + " was " + UpstreamServices.Count);
            //                    List<Service> NewServiceList = GetUpstreamServiceList(UpstreamService, UpstreamServices);

            //                    foreach (Service NewService in NewServiceList)
            //                    {
            //                        if (Global.GetExistingService(NewService, UpstreamServices) == null)
            //                        {
            //                            UpstreamServices.Add(NewService);
            //                        }
            //                    }
            //                    Debug.WriteLine("Upstream service count for " + Service.Name + " is now " + UpstreamServices.Count);


            //                    //Debug.WriteLine(Service.Name + " needs " + DownstreamService.Name);
            //                    //DownstreamServices.Add(DownstreamService);
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            return UpstreamServices;
        }

        private void BuildServiceDependencies(BusinessApplication FocusService, BusinessApplication CurrentlyTargetedService, ref List<BusinessApplication> ListOfServicesAlreadyFound)
        {
            // For each Service, check it's Component Services
            foreach (BusinessApplication ComponentService in CurrentlyTargetedService.ComponentBusinessApplications) //ServiceFromAllServiceList.ComponentServices)
            {
                // ... until we find one that relies on this service
                if (ComponentService == FocusService)
                {
                    // Check to ensure we don't already have it in our list
                    if (Global.GetExistingBusinessApplication(CurrentlyTargetedService, ListOfServicesAlreadyFound) == null)
                    {
                        // Not in the list, so add it to the List of Upstream Services...
                        ListOfServicesAlreadyFound.Add(CurrentlyTargetedService);

                        // check it isn't in our list of Dependent services...
                        if (Global.GetExistingBusinessApplication(CurrentlyTargetedService, FocusService.DependentBusinessApplications) == null)
                        {
                            // .. no it isn't so add it
                            // (But only if it isn't this service)
                            if (FocusService != CurrentlyTargetedService)
                            {
                                Debug.WriteLine(CurrentlyTargetedService.Name + " needs " + FocusService.Name, "information");

                                // This is a relationship, so create it that way
                                //Relationship Relationship = new Dependency.Relationship(ref CurrentlyTargetedService, ref FocusService);
                                //this.Relationships.Add(Relationship);

                                FocusService.DependentBusinessApplications.Add(CurrentlyTargetedService);
                                //Debug.WriteLine("===========================> Added " + CurrentlyTargetedService.Name + " to " + FocusService.Name + " dependencies");

                                // Call ourselves recursively to continue building the List
                                ListOfServicesAlreadyFound = GetUpstreamServiceList(CurrentlyTargetedService, ListOfServicesAlreadyFound);
                            }
                        }
                    }
                }
            }
        }

        private List<BusinessApplication> Dependents(BusinessApplication Service)
        {
            List<BusinessApplication> Result = new List<BusinessApplication>();

            return Result;
        }

        #endregion

        #region Public Methods

        public bool AddRelationship(ref Relationship ThisRelationship)
        {
            // If the relationship is between a Service and a Server...
            if (ThisRelationship.ToType == Dependency.Relationship.ComponentType.Server)
            {
                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    if (ThisRelationship.SystemMandatory)
                    {
                        Debug.WriteLine("......[INF1098] Added Relationship to " + ThisRelationship.ToServer.Name + " (Server)", "information");
                    }
                    else
                    {
                        Debug.WriteLine("......[INF1099] Added Relationship to " + ThisRelationship.ToServer.Name + " (Server) (Optional)", "information");
                    }

                }

                Server ComponentServer = ThisRelationship.ToServer;

                ComponentServer.ID = ThisRelationship.ToObjectID;
                ComponentServer.VersionID = ThisRelationship.VersionID;

                Server ExistingComponentServer = Global.GetExistingServer(ThisRelationship.ToServer, _Servers);

                // This is a new Server
                if (ExistingComponentServer == null)
                {
                    BusinessApplication s = ThisRelationship.FromBusinessApplication;
                    AddServer(ref ComponentServer, ref s);

                    ThisRelationship.FromBusinessApplication = s;

                    BusinessApplication Service = ThisRelationship.FromBusinessApplication;

                    if (ThisRelationship.FromBusinessApplication.InScope || ThisRelationship.FromBusinessApplication.OverrideInScope)
                    {
                        if (ThisRelationship.SystemMandatory)
                        {
                            if (!_TopoSort.AddConnection(ThisRelationship.FromBusinessApplication.Name, ThisRelationship.ToServer.Name))
                            {
                                Errors.Add("[ERR1001] Error adding connection between " + ThisRelationship.FromBusinessApplication.Name + " (Business Application) and new " + ThisRelationship.ToServer.Name + " (Server)");
                                //Debug.WriteLine("[ERR1001] Error adding connection between " + ThisRelationship.FromBusinessApplication.Name + " (Business Application) and new " + ThisRelationship.ToServer.Name + " (Server)");
                                return false;
                            }
                        }
                    }
                }
                // This is an existing Server
                else
                {
                    BusinessApplication Service = ThisRelationship.FromBusinessApplication;

                    ComponentServer = ExistingComponentServer;

                    if (ThisRelationship.FromBusinessApplication.InScope || ThisRelationship.FromBusinessApplication.OverrideInScope)
                    {
                        if (ThisRelationship.SystemMandatory)
                        {
                            if (!_TopoSort.AddConnection(ThisRelationship.FromBusinessApplication.Name, ExistingComponentServer.Name))
                            {
                                Errors.Add("[ERR1002] Error adding connection between " + ThisRelationship.FromBusinessApplication.Name + " (Business Application) and existing " + ExistingComponentServer.Name + " (Server)");
                                return false;
                            }
                        }
                    }
                }
            }
            // If the relationship is between a Service and another Service...
            else if (ThisRelationship.ToType == Dependency.Relationship.ComponentType.BusinessApplication)
            {
                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine("......[INF1097] Added Relationship to " + ThisRelationship.ToBusinessApplication.Name + " (Business Application)", "information");
                }

                BusinessApplication Service = ThisRelationship.FromBusinessApplication;
                BusinessApplication ComponentService = ThisRelationship.ToBusinessApplication;

                ComponentService.ID = ThisRelationship.ToObjectID;
                ComponentService.VersionID = ThisRelationship.VersionID;

                Service.AddRelationship(ThisRelationship);

                AddService(ref ComponentService);

                if (
                      (ThisRelationship.FromBusinessApplication.InScope || ThisRelationship.FromBusinessApplication.OverrideInScope) &&
                      (ThisRelationship.ToBusinessApplication.InScope || ThisRelationship.ToBusinessApplication.OverrideInScope)
                    )
                {
                    if (ThisRelationship.SystemMandatory)
                    {
                        if (!_TopoSort.AddConnection(ThisRelationship.FromBusinessApplication.Name, ThisRelationship.ToBusinessApplication.Name))
                        {
                            Errors.Add("[ERR1003] Error adding connection between " + ThisRelationship.FromBusinessApplication.Name + " (Business Application) and existing " + ThisRelationship.ToBusinessApplication.Name + " (Server)");
                            return false;
                        }

                        foreach (Server Server in ThisRelationship.ToBusinessApplication.ComponentServers)
                        {
                            if (!_TopoSort.AddConnection(ThisRelationship.FromBusinessApplication.Name, Server.Name))
                            {
                                Errors.Add("[ERR1004] Error adding connection between " + ThisRelationship.FromBusinessApplication.Name + " (Business Application) and existing " + ThisRelationship.ToBusinessApplication.Name + " (Server)");
                                return false;
                            }
                        }
                    }
                }
            }

            _Relationships.Add(ThisRelationship);

            return true;
        }

        public void SortRunbook()
        {
            _TopoSort.Sort(out _Runbook);
        }

        public List<BusinessApplication> RunbookServices()
        {
            List<BusinessApplication> Services = new List<BusinessApplication>();

            // Runbook now contains a list of strings, representing Servers and Services
            // This needs to be filtered and Servers removed
            foreach (string ServerOrServiceName in _Runbook)
            {
                BusinessApplication Service = Global.GetExistingBusinessApplication(ServerOrServiceName, _Services);
                if (Service != null)
                {
                    // Found this Service.  If it is InScope, add it
                    if (Service.InScope || Service.OverrideInScope)
                    {
                        Services.Add(Service);
                    }
                }
            }

            return Services;
        }

        public List<Server> RunbookServers()
        {
            List<BusinessApplication> Services = RunbookServices();
            List<Server> Servers = new List<Server>();

            // Runbook now contains a list of Services.  We have to go through this list and extract out the Servers
            foreach (BusinessApplication Service in Services)
            {
                foreach (Server ComponentServer in Service.ComponentServers)
                {
                    if (Global.GetExistingServer(ComponentServer, Servers) == null)
                    {
                        Servers.Add(ComponentServer);
                    }
                }
            }

            return Servers;
        }

        public void CalculateServiceRunbook(ref List<BusinessApplication> ServiceRunbook)
        {
            List<BusinessApplication> Runbook = new List<BusinessApplication>();
            Int32 RunbookOrder = 1;
            bool IncreasePriorityGroup = false;
            Int32 MinimumRequiredOrder = 1;

            // Reset values
            foreach (BusinessApplication Service in ServiceRunbook)
            {
                Service.RunbookOrder = 1;
            }

            foreach (BusinessApplication Service in ServiceRunbook)
            {
                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine("..." + Service.Name, "information");
                }

                IncreasePriorityGroup = false;

                // Check if this Service uses any of the Services already added
                foreach (BusinessApplication ComponentService in Service.ComponentBusinessApplications)
                {
                    if (Runbook.Contains(ComponentService))
                    {
                        Relationship TheRelationship = Global.GetRelationship(Service, ComponentService, _Relationships);

                        if (TheRelationship != null)
                        {
                            if (ComponentService.RunbookOrder >= Service.RunbookOrder && TheRelationship.SystemMandatory)
                            {
                                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                                {
                                    Debug.WriteLine("......[INF1005] Runbook Index increased due to " + ComponentService.Name + " (" + ComponentService.RunbookOrder + ")", "information");
                                }
                                IncreasePriorityGroup = true;

                                if (ComponentService.RunbookOrder > MinimumRequiredOrder)
                                {
                                    MinimumRequiredOrder = ComponentService.RunbookOrder;
                                }
                            }
                        }
                        else
                        {
                            Errors.Add("[ERR1006] Relationship between " + Service.Name + " (Business Application) and " + ComponentService.Name + " (Business Application) could not be found.  This should not occur!");
                            Debug.WriteLine("......[ERR1006] Relationship between " + Service.Name + " (Business Application) and " + ComponentService.Name + " (Business Application) could not be found.  This should not occur!", "error");
                        }
                    }
                }

                if (IncreasePriorityGroup)
                {
                    // The Order for this Service has to be increased, but does it have to be greater than the order we are currently working with?

                    if (MinimumRequiredOrder >= RunbookOrder)
                    {
                        RunbookOrder++;
                    }
                }

                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine("......[INF1089] Runbook Order set to " + RunbookOrder, "information");
                }

                Service.RunbookOrder = RunbookOrder;

                if (VaderConsulting.Helper.Properties.ShowDebugInformation && Service.ComponentServers.Count > 0)
                {
                    Debug.WriteLine("......Setting Component Servers...", "information");
                }

                foreach (Server Server in Service.ComponentServers)
                {
                    if (Server.SRMRecoveryIndex >= Service.RunbookOrder)
                    {
                        Server.SRMRecoveryIndex = Service.RunbookOrder;

                        if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                        {
                            Debug.WriteLine(".........[INF1028] " + Server.Name + ": Runbook Order set to " + RunbookOrder, "information");
                        }
                    }
                }

                Runbook.Add(Service);
            }

            if (VaderConsulting.Helper.Properties.ShowDebugInformation)
            {
                Debug.WriteLine("...Final result...", "information");
                foreach (BusinessApplication Service in Runbook)
                {
                    Debug.WriteLine("......" + Service.Name + ": " + Service.RunbookOrder, "information");
                }
            }

            return; // Runbook;
        }

        //public List<string> RunbookMasterSRM()
        //{
        //    List<Server> Servers = RunbookServers();
        //    List<BusinessApplication> Services = RunbookServices();
        //    List<string> Runbook = new List<string>();
        //    Int32 PriorityGroupID = 0;
        //    Int32 RecoveryPlanID = 1;
        //    bool SelfProvidedService = false;
        //    bool IncreasePriorityGroup = false;

        //    foreach (Server Server in Servers)
        //    {
        //        Global.RecoveryTier ThisTier = Global.RecoveryTier.Undefined;

        //        Server.SRMRecoveryPlanID = RecoveryPlanID;
        //        Server.SRMPriorityGroupID = PriorityGroupID;

        //        // -----------------------------------------------------------
        //        // Get Services provided by this Server
        //        foreach (BusinessApplication ProvidedService in Server.ProvidedBusinessApplications)
        //        {
        //            if (ProvidedService.Tier < ThisTier)
        //                ThisTier = ProvidedService.Tier;

        //            if (ProvidedService.ComponentBusinessApplications != null && ProvidedService.ComponentBusinessApplications.Count > 0)
        //            {
        //                // Get Services that this service needs
        //                foreach (BusinessApplication ComponentService in ProvidedService.ComponentBusinessApplications)
        //                {
        //                    SelfProvidedService = false;

        //                    if (ComponentService.Flag != true)  // This is used as a flag so that we know that the Service has been ordered already
        //                    {
        //                        // Get Servers that provide this service
        //                        foreach (Server ThisServer in ComponentService.ComponentServers)
        //                        {
        //                            // First go through the Servers for this Service to determine if the Service is self-hosted
        //                            foreach (Server ComponentServer in ComponentService.ComponentServers)
        //                            {
        //                                if (ComponentServer.Name == Server.Name)
        //                                {
        //                                    // Used to track dependencies with itself (i.e. self-hosted).
        //                                    // For example, AD relies upon DNS, but both Services can be provided by the same Server
        //                                    SelfProvidedService = true;
        //                                    break;
        //                                }
        //                            }

        //                            // Now we know if the Service is self-hosted or not, we can determine if we need to modify it's Priority Group
        //                            if (!SelfProvidedService)
        //                            {
        //                                foreach (Server ComponentServer in ComponentService.ComponentServers)
        //                                {
        //                                    float ServerSRMPlacement = 0.0F;
        //                                    float.TryParse(Server.SRMRecoveryPlanID.ToString() + "." + Server.SRMPriorityGroupID.ToString(), out ServerSRMPlacement);
        //                                    float ComponentServerSRMPlacement = 0.0F;
        //                                    float.TryParse(ComponentServer.SRMRecoveryPlanID.ToString() + "." + ComponentServer.SRMPriorityGroupID.ToString(), out ComponentServerSRMPlacement);

        //                                    // Check to see it is adequate
        //                                    if (ServerSRMPlacement <= ComponentServerSRMPlacement)
        //                                    {
        //                                        // This is a problem.  The Dependent service must be started up before this service
        //                                        //Debug.WriteLine(ProvidedService.Name + "------>" + Server.Name + ": " + ServerSRMPlacement + "    " + ComponentServer.Name + ": " + ComponentServerSRMPlacement + " ... Increasing Server placement");
        //                                        IncreasePriorityGroup = true;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        //foreach (BusinessApplication ThisService in ComponentService.ComponentBusinessApplications)
        //                        //{
        //                        //    foreach (Server ComponentServer in ComponentService.ComponentServers)
        //                        //    {
        //                        //        float ServerSRMPlacement = 0.0F;
        //                        //        float.TryParse(Server.SRMRecoveryPlanID.ToString() + "." + Server.SRMPriorityGroupID.ToString(), out ServerSRMPlacement);
        //                        //        float ComponentServerSRMPlacement = 0.0F;
        //                        //        float.TryParse(ComponentServer.SRMRecoveryPlanID.ToString() + "." + ComponentServer.SRMPriorityGroupID.ToString(), out ComponentServerSRMPlacement);

        //                        //        // Check to see it is adequate
        //                        //        if (ServerSRMPlacement <= ComponentServerSRMPlacement)
        //                        //        {
        //                        //            // This is a problem.  The Dependent service must be started up before this service
        //                        //            //Debug.WriteLine(ProvidedService.Name + "------>" + Server.Name + ": " + ServerSRMPlacement + "    " + ComponentServer.Name + ": " + ComponentServerSRMPlacement + " ... Increasing Server placement");
        //                        //            IncreasePriorityGroup = true;
        //                        //        }
        //                        //    }
        //                        //}
        //                        // Now we know this Service needs to be started up, set the flag so we know we have addressed it
        //                        ComponentService.Flag = true;
        //                    }
        //                }
        //            }

        //            foreach (BusinessApplication ThisService in ProvidedService.ComponentBusinessApplications)
        //            {
        //                foreach (Server ComponentServer in ThisService.ComponentServers)
        //                {
        //                    float ServerSRMPlacement = 0.0F;
        //                    float.TryParse(Server.SRMRecoveryPlanID.ToString() + "." + Server.SRMPriorityGroupID.ToString(), out ServerSRMPlacement);
        //                    float ComponentServerSRMPlacement = 0.0F;
        //                    float.TryParse(ComponentServer.SRMRecoveryPlanID.ToString() + "." + ComponentServer.SRMPriorityGroupID.ToString(), out ComponentServerSRMPlacement);

        //                    // Check to see it is adequate
        //                    if (ServerSRMPlacement <= ComponentServerSRMPlacement)
        //                    {
        //                        // This is a problem.  The Dependent service must be started up before this service
        //                        //Debug.WriteLine(ProvidedService.Name + "------>" + Server.Name + ": " + ServerSRMPlacement + "    " + ComponentServer.Name + ": " + ComponentServerSRMPlacement + " ... Increasing Server placement");
        //                        IncreasePriorityGroup = true;
        //                    }
        //                }
        //            }

        //        }

        //        Server.Tier = ThisTier;

        //        if (IncreasePriorityGroup)
        //        {
        //            PriorityGroupID++;
        //            Debug.WriteLine(Server.Name + " Priority group increased");

        //            if (PriorityGroupID > 5)
        //            {
        //                RecoveryPlanID++;
        //                PriorityGroupID = 1;
        //                Debug.WriteLine("Recovery Plan increased");
        //            }
        //        }

        //        Server.SRMRecoveryPlanID = RecoveryPlanID;
        //        Server.SRMPriorityGroupID = PriorityGroupID;

        //        // Now we know the RecoveryPlan and PriorityGroup of this Server, we must set the same values to all Services provided by the Server
        //        // If the Service already has a value set, only update it if the new value is lower than the existing value
        //        foreach (BusinessApplication Service in Server.ProvidedBusinessApplications)
        //        {
        //            if (Service.CalculatedSRMRecoveryPlanID > Server.SRMRecoveryPlanID ||
        //                Service.CalculatedSRMPriorityGroupID > Server.SRMPriorityGroupID)
        //            {
        //                Service.CalculatedSRMRecoveryPlanID = Server.SRMRecoveryPlanID;
        //                Service.CalculatedSRMPriorityGroupID = Server.SRMPriorityGroupID;
        //                Debug.WriteLine(Service.Name + ": SRM order set to " + Server.SRMRecoveryPlanID + "." + Server.SRMPriorityGroupID);
        //            }
        //        }

        //        IncreasePriorityGroup = false;
        //        SelfProvidedService = false;
        //    }

        //    //Debug.WriteLine("Final result:");

        //    foreach (Server Server in Servers)
        //    {
        //        //Debug.WriteLine(Server.Name + " = " + Server.SRMRecoveryPlanID + ", " + Server.SRMPriorityGroupID);
        //        Runbook.Add(Server.Name + " = " + Server.SRMRecoveryPlanID + ", " + Server.SRMPriorityGroupID);
        //    }

        //    return Runbook;
        //}

        //private bool IncreasePriorityOrder(BusinessApplication BusinessApplication, BusinessApplication ComparisonBusinessApplication)
        //{
        //    bool Result = false;

        //    // Go through the Services Component Business Applications
        //    foreach (BusinessApplication Service in BusinessApplication.ComponentBusinessApplications)
        //    {
        //        float ServerSRMPlacement = 0.0F;
        //        float.TryParse(BusinessApplication.CalculatedSRMRecoveryPlanID.ToString() + "." + BusinessApplication.CalculatedSRMPriorityGroupID.ToString(), out ServerSRMPlacement);
        //        float ComponentServerSRMPlacement = 0.0F;
        //        float.TryParse(ComponentServer.SRMRecoveryPlanID.ToString() + "." + ComponentServer.SRMPriorityGroupID.ToString(), out ComponentServerSRMPlacement);

        //        // Check to see it is adequate
        //        if (ServerSRMPlacement <= ComponentServerSRMPlacement)
        //        {
        //            // This is a problem.  The Dependent service must be started up before this service
        //            //Debug.WriteLine(ProvidedService.Name + "------>" + Server.Name + ": " + ServerSRMPlacement + "    " + ComponentServer.Name + ": " + ComponentServerSRMPlacement + " ... Increasing Server placement");
        //            IncreasePriorityGroup = true;
        //        }
        //    }

        //    // Now go through the Services Component Servers
        //    foreach (Server ThisServer in BusinessApplication.ComponentServers)
        //    {
        //    }

        //    return Result;
        //}

        private Global.RecoveryStream CalculateServiceStream(ref BusinessApplication Service)
        {
            Global.RecoveryStream ComponentServerStream = Global.RecoveryStream.Undefined;
            Global.RecoveryStream RolledUpComponentServiceStream = Global.RecoveryStream.Undefined;

            if (Service.CalculatedStream != Global.RecoveryStream.Undefined)
            {
                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    // Stream has previously been defined, so just return that value
                    Debug.WriteLine(".........[INF1036] " + Service.Name + " (Business Application) Stream is " + Global.GetBusinessApplicationStreamName(Service.CalculatedStream), "information");
                }
                return Service.CalculatedStream;
            }


            if (Service.MandatoryServers.Count > 0)
            {
                ComponentServerStream = Service.MandatoryServers.OrderBy(s => s.Stream)
                                                                .FirstOrDefault().Stream;

                //ComponentServerStream = Service.ComponentServers.Where(s => s.InScope == true).OrderBy(s => s.Stream).FirstOrDefault().Stream;
            }
            else
            {
                ComponentServerStream = Global.RecoveryStream.Undefined; // No Component Servers, so the resultant here is 'Undefined'
            }

            if (Service.MandatoryBusinessApplications.Count > 0)
            {
                foreach (BusinessApplication ComponentService in Service.MandatoryBusinessApplications)
                {
                    if (ComponentService.InScope)
                    {
                        BusinessApplication c = ComponentService;
                        RolledUpComponentServiceStream = CalculateServiceStream(ref c);
                        //int y = 1;
                    }
                }
            }
            else
            {
                RolledUpComponentServiceStream = Global.RecoveryStream.Undefined; // No Component Services, so the resultant here is 'Undefined'
            }

            if (ComponentServerStream > RolledUpComponentServiceStream)
            {
                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine(".........[INF1090] " + Service.Name + " (Business Application) Stream is " + Global.GetBusinessApplicationStreamName(ComponentServerStream), "information");
                }

                return ComponentServerStream;
            }
            else
            {
                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine(".........[INF1091] " + Service.Name + " (Business Application) Stream is " + Global.GetBusinessApplicationStreamName(RolledUpComponentServiceStream), "information");
                }

                return RolledUpComponentServiceStream;
            }
        }

        public void SetServiceStream(ref List<BusinessApplication> Services)
        {
            for (int i = 0; i < Services.Count; i++)
            {
                BusinessApplication s = Services[i];
                SetServiceStream(ref s);
                Services[i] = s;
            }

            //foreach (BusinessApplication Service in Services)
            //{
            //    BusinessApplication s = Service;
            //    SetServiceStream(ref s);
            //    Service = s;
            //}
        }

        private void SetServiceStream(ref BusinessApplication Service)
        {
            Server ComponentServer = null;
            Global.RecoveryStream ComponentServerStream = Global.RecoveryStream.Undefined;
            Global.RecoveryStream ComponentServiceStream = Global.RecoveryStream.Undefined;
            Global.RecoveryStream RolledUpComponentServiceStream = Global.RecoveryStream.Undefined;
            Global.RecoveryStream HAServiceStream = Global.RecoveryStream.Undefined;

            if (VaderConsulting.Helper.Properties.ShowDebugInformation)
            {
                Debug.WriteLine("..." + Service.Name + "...", "information");
            }

            // Set a default
            Service.CalculatedStream = RolledUpComponentServiceStream;

            if (Service.MandatoryServers.Count > 0)
            {
                #region Rollup all Servers

                //if (Service.Name == "ADFS")
                //{
                //    int i = 1;
                //}

                ComponentServer = Service.MandatoryServers.OrderByDescending(s => s.Stream)
                                                          .FirstOrDefault();

                if (ComponentServer != null)
                {
                    ComponentServerStream = ComponentServer.Stream;
                }

                #endregion
            }
            else
            {
                ComponentServerStream = Global.RecoveryStream.Undefined; // No Component Servers, so the resultant here is 'Undefined'
            }

            if (VaderConsulting.Helper.Properties.ShowDebugInformation)
            {
                Debug.WriteLine("......[INF1092] Rolled up Server Stream is " + ComponentServerStream.ToString(), "information");
            }

            if (Service.MandatoryBusinessApplications.Count > 0)
            {
                #region Rollup all Services

                // Perform a LINQ query to see if we would receive any results
                List<BusinessApplication> OrderedList = (from s in Service.MandatoryBusinessApplications
                                                     orderby s.CalculatedStream descending
                                                     where s.InScope == true
                                                     select s).ToList();

                if (OrderedList.Count == 0)
                {
                    ComponentServiceStream = Global.RecoveryStream.Undefined;
                }
                else
                {
                    // Perform the same query, but this time get the Calculated Stream from the first Service in the list
                    ComponentServiceStream = (from s in Service.MandatoryBusinessApplications
                                              orderby s.CalculatedStream descending
                                              where s.InScope == true
                                              select s)
                                              .FirstOrDefault().CalculatedStream;

                    RolledUpComponentServiceStream = ComponentServiceStream;
                }

                #endregion
            }
            else
            {
                RolledUpComponentServiceStream = Global.RecoveryStream.Undefined; // No Component Services, so the resultant here is 'Undefined'
            }

            if (VaderConsulting.Helper.Properties.ShowDebugInformation)
            {
                Debug.WriteLine("......[INF1093] Rolled up Business Application Stream is " + RolledUpComponentServiceStream, "information"); //Global.GetServiceStreamName(ComponentServiceStream));
            }

            #region Compare Rolled up Services to Rolled up Servers

            if (Service.ComponentServers.Count > 0)
            {
                if (ComponentServerStream > RolledUpComponentServiceStream)
                {
                    Service.CalculatedStream = ComponentServerStream;

                    if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                    {
                        Debug.WriteLine("......[INF1094] Resultant Stream is " + Service.CalculatedStream, "information"); //Global.GetServiceStreamName(Service.CalculatedStream));
                    }
                }
                else
                {
                    Service.CalculatedStream = RolledUpComponentServiceStream;

                    if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                    {
                        Debug.WriteLine("......[INF1095] Resultant Stream is " + Service.CalculatedStream, "information"); //Global.GetServiceStreamName(Service.CalculatedStream));
                    }
                }
            }
            else // No component servers
            {
                Service.CalculatedStream = RolledUpComponentServiceStream;

                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine("......[INF1096] Resultant Stream is " + Service.CalculatedStream, "information"); //Global.GetServiceStreamName(Service.CalculatedStream));
                }
            }

            #endregion

            #region HA

            // Now handle HA Services

            bool TreatAsHA = false;

            if (Service.HighlyAvailable || Service.HARelationships.Count > 0 || Service.HAServers.Count > 0)
            {
                TreatAsHA = true;
            }

            if (TreatAsHA)
            {
                // Are there any Component Servers or Services that are not HA?
                foreach (Relationship r in Service.Relationships)
                {
                    if (r.SystemMandatory || r.UserMandatory)
                    {
                        switch (r.ToType)
                        {
                            case Relationship.ComponentType.BusinessApplication:
                                if (r.ToBusinessApplication.HighlyAvailable == false && r.StreamDependency == false)
                                {
                                    Errors.Add("[ERR1007] " + Service.Name + " (Business Application) cannot be HA, so has been temporarily set to non-HA.  This is due to " + r.ToBusinessApplication.Name + " (Business Application) not being Highly Available");
                                    Debug.WriteLine("......[ERR1007] " + Service.Name + " (Business Application) cannot be HA, so has been temporarily set to non-HA.  This is due to " + r.ToBusinessApplication.Name + " (Business Application) not being Highly Available", "warning");
                                    //Service.HighlyAvailable = false;
                                    TreatAsHA = false;
                                }
                                break;
                        }
                    }
                }
            }

            if (TreatAsHA)
            {
                //VaderConsulting.Dependency.Global.RecoveryStream ProtectedSiteResult = VaderConsulting.Dependency.Global.RecoveryStream.Undefined;
                //VaderConsulting.Dependency.Global.RecoveryStream RecoverySiteResult = VaderConsulting.Dependency.Global.RecoveryStream.Undefined;
                //VaderConsulting.Dependency.Global.RecoveryStream ExternalSiteResult = VaderConsulting.Dependency.Global.RecoveryStream.Undefined;
                //VaderConsulting.Dependency.Global.RecoveryStream NotProtectedSiteResult = VaderConsulting.Dependency.Global.RecoveryStream.Undefined;
                //VaderConsulting.Dependency.Global.RecoveryStream OverallResult = VaderConsulting.Dependency.Global.RecoveryStream.Undefined;

                foreach (Relationship Relationship in Service.Relationships) // Relationship is Service <-> Service
                {
                    Global.RecoveryStream ResultantRelationshipStream = Service.CalculatedStream;

                    // This can only be done where the realtionship is not optional and not because of a stream dependency
                    if (Relationship.SystemMandatory && !Relationship.StreamDependency)
                    {
                        // Get any HA relationships between this component and the service
                        if (Relationship.ToType == VaderConsulting.Dependency.Relationship.ComponentType.BusinessApplication)
                        {
                            // At this time, there is no capability to add a HA relationship between Services, so the resultant stream is mainly dependenat upon the initial stream
                            ResultantRelationshipStream = Relationship.ToBusinessApplication.CalculatedStream;
                        }
                        else // Relationship is Server <-> Server
                        {
                            // Get the Server that is in this Relationship
                            Server Server = Relationship.ToServer;

                            // Now check if it has any HA Relationships
                            if (Server.HARelationships.Count > 0)
                            {
                                // Now we know that there MAY be a HA relationship, we need to check
                                foreach (HARelationship HARelationship in Server.HARelationships)
                                {
                                    if (HARelationship.Service == Service)
                                    {
                                        // Ok, so this Server has a HA Relationship with this Service.  

                                        // Check we don't already have it
                                        if (!Service.HAServers.Contains(HARelationship.Server1))
                                        {
                                            //Add it to our list of HA Servers
                                            Service.HAServers.Add(HARelationship.Server1);
                                        }

                                        // This HA Relationship with this Service has 2 Servers.  

                                        // Check we don't already have the 2nd
                                        if (!Service.HAServers.Contains(HARelationship.Server2))
                                        {
                                            //Add it to our list of HA Servers
                                            Service.HAServers.Add(HARelationship.Server2);
                                        }

                                        // ================================================================
                                        if (!Service.HARelationships.Contains(HARelationship))
                                        {
                                            Service.HARelationships.Add(HARelationship);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // There may be multiple groups of HA Servers (think CommVault Media Servers and CommVault Witness Servers)
                bool FoundRecoverySite = false;
                bool FoundProtectedSite = false;
                //bool FoundNotProtectedSite = false;
                bool FoundExternalSite = false;

                foreach (Server Server in Service.HAServers)
                {
                    // Get the relationship between this Server and the Service
                    Relationship Relationship = Global.GetRelationship(Service, Server, Server.Relationships);

                    #region Protected

                    if (Server.PhysicalSite.SiteType == PhysicalSite.PhysicalSiteType.Protected)
                    {
                        FoundProtectedSite = true;
                    }

                    #endregion

                    #region Recovery

                    if (Server.PhysicalSite.SiteType == PhysicalSite.PhysicalSiteType.Recovery)
                    {
                        FoundRecoverySite = true;
                    }

                    #endregion

                    #region Not Protected

                    //if (Server.PhysicalSite.SiteType == PhysicalSite.PhysicalSiteType.NotProtected)
                    //{
                    //    FoundNotProtectedSite = true;
                    //}

                    #endregion

                    #region External

                    if (!Server.PhysicalSite.Internal)
                    {
                        FoundExternalSite = true;
                    }

                    #endregion


                    // Now that we have worked out the Resultant Steam, we need to apply it to the Server <-> Service relationship
                    Relationship NonHARelationship = Global.GetRelationship(Service, Server, Service.Relationships);

                    if (NonHARelationship != null)
                    {
                        NonHARelationship.ResultantStream = Relationship.ResultantStream;
                    }
                    else
                    {
                        Errors.Add("[ERR1008] Could not find relationship between " + Service.Name + " (Business Application) and " + Server.Name + " (Server).  This should not occur.");
                        Debug.WriteLine("***** [ERR1008] Could not find relationship between " + Service.Name + " (Business Application) and " + Server.Name + " (Server).  This should not occur.", "error");
                    }
                }

                // Work out the resultant stream, taking HA into account
                if (Service.HARelationships.Count > 0)
                {

                    if (!FoundProtectedSite)
                    {
                        if (!FoundRecoverySite)
                        {
                            if (!FoundExternalSite)
                            {
                                //AddDrawingException(Service.Name + " (Business Application) is incorrectly marked as HA", Service);

                                // Determine Stream by rolling up the Server Stream
                            }
                            else
                            {
                                //AddDrawingException(Service.Name + " (Business Application) is incorrectly marked as HA", Service);

                                HAServiceStream = Global.RecoveryStream.None1;
                            }
                        }
                        else
                        {
                            //AddDrawingException(Service.Name + " is incorrectly marked as HA", Service);

                            HAServiceStream = Global.RecoveryStream.None2;
                        }
                    }
                    else
                    {
                        if (FoundRecoverySite)
                        {
                            HAServiceStream = Global.RecoveryStream.None3; // This is the desired result
                        }
                        else
                        {
                            Errors.Add("[ERR1199] " + Service.Name + " (Business Application) is incorrectly marked as HA");
                            Debug.WriteLine("......[ERR1199] " + Service.Name + " (Business Application) is incorrectly marked as HA", "warning");
                        }
                    }
                }

                Service.CalculatedStream = HAServiceStream;
            }

            #endregion

            if (Helper.Properties.ShowDebugInformation)
            {
                Debug.WriteLine("......[INF1200] Final Stream is " + Service.CalculatedStream, "information");
            }
        }

        public List<string> RunbookServiceSRM()
        {
            List<Server> Servers = RunbookServers();
            List<BusinessApplication> Services = RunbookServices();
            List<string> Runbook = new List<string>();
            Int32 SRMRecoveryIndex = 1;
            bool SelfProvidedService = false;
            bool IncreasePriorityGroup = false;

            foreach (Server Server in Servers)
            {
                Server.SRMRecoveryIndex = SRMRecoveryIndex;

                // -----------------------------------------------------------
                // Get Services provided by this Server
                foreach (BusinessApplication ProvidedService in Server.ProvidedBusinessApplications)
                {
                    // Get Services that this service needs
                    foreach (BusinessApplication ComponentService in ProvidedService.ComponentBusinessApplications)
                    {
                        SelfProvidedService = false;

                        if (ComponentService.Flag != true)
                        {
                            // Get Servers that provide this service
                            foreach (Server ThisServer in ComponentService.ComponentServers)
                            {

                                // First go through the Servers for this Service to determine if the Service is self-hosted
                                foreach (Server ComponentServer in ComponentService.ComponentServers)
                                {
                                    if (ComponentServer.Name == Server.Name)
                                    {
                                        // Used to track dependencies with itself (i.e. self-hosted).
                                        // For example, AD relies upon DNS, but both Services can be provided by the same Server
                                        SelfProvidedService = true;
                                        break;
                                    }
                                }

                                // Now we know if the Service is self-hosted or not, we can determine if we need to modify it's Priority Group
                                if (!SelfProvidedService)
                                {
                                    foreach (Server ComponentServer in ComponentService.ComponentServers)
                                    {
                                        // Check to see it is adequate
                                        if (Server.SRMRecoveryIndex <= SRMRecoveryIndex)
                                        {
                                            // This is a problem.  The Dependent service must be started up before this service
                                            IncreasePriorityGroup = true;
                                        }
                                    }
                                }
                            }
                            // Now we know this Service needs to be started up, set the flag so we know we have addressed it
                            ComponentService.Flag = true;
                        }
                    }
                }

                if (IncreasePriorityGroup)
                {
                    SRMRecoveryIndex++;
                }

                Server.SRMRecoveryIndex = SRMRecoveryIndex;

                IncreasePriorityGroup = false;
                SelfProvidedService = false;
            }

            foreach (Server Server in Servers)
            {
                Runbook.Add(Server.Name + " = " + Server.SRMRecoveryIndex);
            }

            return Runbook;
        }

        public void RemoveRelationship(ref Relationship ThisRelationship)
        {
            if (ThisRelationship.ToType == Dependency.Relationship.ComponentType.Server)
            {
                RemoveServer(ThisRelationship.ToServer);
            }
            else
            {
                RemoveService(ThisRelationship.ToBusinessApplication);
            }

            _Relationships.Remove(ThisRelationship);
        }

        public void DetermineUpstreamServices(ref Server Server)
        {
            List<BusinessApplication> UpstreamServices = new List<BusinessApplication>();

            foreach (BusinessApplication Service in Server.ProvidedBusinessApplications)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Add the immediate Services that are reliant upon this Server
                if (Global.GetExistingBusinessApplication(Service, Server.DependentBusinessApplications) == null)
                {
                    Server.DependentBusinessApplications.Add(Service);
                }

                if (Global.GetExistingBusinessApplication(Service, UpstreamServices) == null) // && (UpstreamService.Name != Service.Name))
                {
                    UpstreamServices.Add(Service);
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                UpstreamServices = GetUpstreamServiceList(Service, UpstreamServices);

                // This ComponentServer will probably already have a list of DependentServices, so add to this list
                foreach (BusinessApplication UpstreamService in UpstreamServices)
                {
                    if (Global.GetExistingBusinessApplication(UpstreamService, Server.DependentBusinessApplications) == null) // && (UpstreamService.Name != Service.Name))
                    {
                        Server.DependentBusinessApplications.Add(UpstreamService);
                    }
                }
            }
        }

        #endregion

    }
}
