using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class Server : ComponentBase
    {

        #region Fields

        private Global.AvailabilityPredictionMethod _AvailabilityPredictionMethod = Global.AvailabilityPredictionMethod.Time;
        private int _CPUCount = 0;
        private List<BusinessApplication> _DependentBusinessApplications = new List<BusinessApplication>();
        private List<Disk> _Disks = new List<Disk>();
        private List<Server> _HASiblings = new List<Server>();
        private List<System.Net.IPAddress> _IPAddressList = new List<System.Net.IPAddress>();
        private Int32 _Memory = 0;
        private string _OperatingSystem = "";
        private bool _OverrideInScope = false;
        private string _PhysicalSiteName = "Unknown Site";
        private PhysicalSite _PhysicalSite = null;
        private List<BusinessApplication> _ProvidedBusinessApplications = new List<BusinessApplication>();
        private List<RecoveryTask> _RecoveryTasks = new List<RecoveryTask>();
        private TimeSpan _RecoveryTime = new TimeSpan(21, 0, 0, 0);
        private List<Relationship> _Relationships = new List<Relationship>();
        private VaderConsulting.Database.SQLServer _SQL = null;
        private int _SRMRecoveryPlanIndex = Int16.MaxValue;
        private Global.RecoveryStream _Stream = Global.RecoveryStream.Undefined;
        private Global.RecoveryTier _Tier = Global.RecoveryTier.Three;
        private Int32 _TotalAllocatedDisk = 0;
        private string _VCenterDescription = "";
        private string _VCenterName = "";
        private bool _Virtual = true;
        private bool _AddToManagementPack = false;
        private List<HARelationship> _HARelationships = new List<HARelationship>();
        private List<BusinessApplication> _HAServices = new List<BusinessApplication>();
        private string _Comment1 = "";
        private string _Comment2 = "";
        private string _Comment3 = "";
        private string _Comment4 = "";
        private string _Comment5 = "";

        #endregion

        #region Properties

        public Global.AvailabilityPredictionMethod AvailabilityPredictionMethod
        {
            get
            {
                return _AvailabilityPredictionMethod;
            }
            set
            {
                _AvailabilityPredictionMethod = value;
            }
        }

        public int CPUCount
        {
            get
            {
                return _CPUCount;
            }
            set
            {
                _CPUCount = value;
            }
        }

        public List<BusinessApplication> DependentBusinessApplications
        {
            get
            {
                return _DependentBusinessApplications;
            }
            set
            {
                _DependentBusinessApplications = value;
            }
        }

        public List<Disk> Disks
        {
            get
            {
                return _Disks;
            }
            set
            {
                _Disks = value;

                _Disks = _Disks.OrderBy(Disk => Disk.Name).ToList();
            }
        }

        public List<HARelationship> HARelationships
        {
            get
            {
                return _HARelationships;
            }
        }

        public List<BusinessApplication> HAServices
        {
            get
            {
                return _HAServices;
            }
            set
            {
                _HAServices = value;
            }
        }

        public List<Server> HASiblings
        {
            get
            {
                return _HASiblings;
            }
            set
            {
                _HASiblings = value;
            }
        }

        public List<System.Net.IPAddress> IPAddressList
        {
            get
            {
                return _IPAddressList;
            }
            set
            {
                _IPAddressList = value;
            }
        }

        public Int32 Memory
        {
            get
            {
                return _Memory;
            }
            set
            {
                _Memory = value;
            }
        }

        public string OperatingSystem
        {
            get
            {
                return _OperatingSystem;
            }
            set
            {
                _OperatingSystem = value;
            }
        }

        //public string PhysicalSiteName
        //{
        //    get
        //    {
        //        return _PhysicalSiteName;
        //    }
        //    set
        //    {
        //        _PhysicalSiteName = value;
        //    }
        //}

        public PhysicalSite PhysicalSite
        {
            get
            {
                return _PhysicalSite;
            }

            set
            {
                _PhysicalSite = value;
            }
        }

        public List<BusinessApplication> ProvidedBusinessApplications
        {
            get
            {
                return _ProvidedBusinessApplications;
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

        public TimeSpan RecoveryTime
        {
            get
            {
                return _RecoveryTime;
            }
            set
            {
                _RecoveryTime = value;
            }
        }

        public List<Relationship> Relationships
        {
            get
            {
                return _Relationships;
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

        public int SRMRecoveryIndex
        {
            get
            {
                return _SRMRecoveryPlanIndex;
            }
            set
            {
                _SRMRecoveryPlanIndex = value;
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

        public Int32 TotalAllocatedDisk
        {
            get
            {
                return _TotalAllocatedDisk;
            }
            set
            {
                _TotalAllocatedDisk = value;
            }
        }

        public string VCenterDescription
        {
            get
            {
                return _VCenterDescription;
            }
            set
            {
                _VCenterDescription = value;
            }
        }

        public string VCenterName
        {
            get
            {
                return _VCenterName;
            }
            set
            {
                _VCenterName = value;
            }
        }

        public bool Virtual
        {
            get
            {
                return _Virtual;
            }
            set
            {
                _Virtual = value;
            }
        }

        public bool AddToManagementPack
        {
            get
            {
                return _AddToManagementPack;
            }
            set
            {
                _AddToManagementPack = value;
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

        public Server(string Name)
            : base()
        {
            base.Name = Name;
        }

        public Server(string Name, Global.HealthState State)
        {
            base.Name = Name;
            base.SetSystemHealthState(State, "");
        }

        #endregion

        #region Internal Methods

        //internal void AddProvidedService(string Service)
        //{
        //    AddProvidedService (new Service(Service));
        //}

        //internal void AddProvidedService(Service Service)
        //{
        //    if (!Service.ComponentServers.Exists(x => x.Name == this.Name))
        //    {
        //        Service.ComponentServers.Add(this);
        //    }
        //    _ServicesProvided.Add(Service);
        //}

        //internal void RemoveProvidedService(string Service)
        //{
        //    RemoveProvidedService(new Service(Service));
        //}

        //internal void RemoveProvidedService(Service Service)
        //{
        //    Service ServiceToRemove = _ServicesProvided.Find(x => x.Name == Service.Name);

        //    if (ServiceToRemove != null)
        //    {
        //        _ServicesProvided.Remove(ServiceToRemove);
        //    }
        //}

        #endregion

        #region Public Methods

        public void AddHARelationship(HARelationship Relationship)
        {
            if (!_HAServices.Contains(Relationship.Service))
            {
                _HAServices.Add(Relationship.Service);
            }

            _HARelationships.Add(Relationship);
        }

        public void AddRelationship(Relationship Relationship, List<Relationship> Relationships)
        {
            if (Global.GetExistingRelationship(Relationship, _Relationships) == null)
            {
                if (Global.GetExistingBusinessApplication(Relationship.FromBusinessApplication, this.ProvidedBusinessApplications) == null)
                {
                    _ProvidedBusinessApplications.Add(Relationship.FromBusinessApplication);

                    //Relationship.FromBusinessApplication.AddRelationship(Relationship, Relationship.FromBusinessApplication.Relationships);
                    Relationship.FromBusinessApplication.AddRelationship(Relationship);
                }

                _Relationships.Add(Relationship);
            }
        }

        //public void CalculateRestoreTime(TimeSpan Time1, TimeSpan Time2, int Speed1, int Speed2, TimeSpan Interval, Dependency.Global.AvailabilityPredictionMethod Method)
        //{
        //    TimeSpan Result = new TimeSpan();

        //    // Get the per-server times
        //    if (Method == Global.AvailabilityPredictionMethod.Time)
        //        Result += Time1;

        //    if (Method == Global.AvailabilityPredictionMethod.TimeAndStorage)
        //        Result += Time2;

        //    // Now get the per-disk times
        //    foreach (Disk Disk in _Disks)
        //    {
        //        switch (Method)
        //        {
        //            //case Global.AvailabilityPredictionMethod.Time:
        //            //    Result += Time1;
        //            //    break;
        //            case Global.AvailabilityPredictionMethod.Storage:
        //                double Time3 = (double)Disk.TotalAllocatedGB / Speed1; // minutes and decimal seconds
        //                Result += TimeSpan.FromMinutes(Time3);
        //                break;
        //            case Global.AvailabilityPredictionMethod.TimeAndStorage:
        //                //Result += Time2;
        //                double Time4 = (double)Disk.TotalAllocatedGB / Speed2; // minutes and decimal seconds
        //                Result += TimeSpan.FromMinutes(Time4);
        //                break;
        //        }
        //    }

        //    _RecoveryTime = Result + Interval;
        //}

        public void CalculateRecoveryTasks()
        {
            TimeSpan TotalTime = TimeSpan.FromSeconds(0);

            RecoveryTask Task = null;

            // Add the disk restore times together
            foreach (Disk Disk in _Disks)
            {
                switch (_Stream)
                {
                    case Global.RecoveryStream.A:
                        if (VaderConsulting.Helper.Properties.StreamAServiceName.ToUpper() != "[NONE]")
                        {
                            Task = new RecoveryTask(base.Name + ": Use " + VaderConsulting.Helper.Properties.StreamAServiceName + " to restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        else
                        {
                            Task = new RecoveryTask(base.Name + ": Restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        break;
                    case Global.RecoveryStream.B:
                        if (VaderConsulting.Helper.Properties.StreamBServiceName.ToUpper() != "[NONE]")
                        {
                            Task = new RecoveryTask(base.Name + ": Use " + VaderConsulting.Helper.Properties.StreamBServiceName + " to restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        else
                        {
                            Task = new RecoveryTask(base.Name + ": Restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        break;
                    case Global.RecoveryStream.C:
                        if (VaderConsulting.Helper.Properties.StreamCServiceName.ToUpper() != "[NONE]")
                        {
                            Task = new RecoveryTask(base.Name + ": Use " + VaderConsulting.Helper.Properties.StreamCServiceName + " to restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        else
                        {
                            Task = new RecoveryTask(base.Name + ": Restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        break;
                    case Global.RecoveryStream.D:
                        if (VaderConsulting.Helper.Properties.StreamDServiceName.ToUpper() != "[NONE]")
                        {
                            Task = new RecoveryTask(base.Name + ": Use " + VaderConsulting.Helper.Properties.StreamDServiceName + " to restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        else
                        {
                            Task = new RecoveryTask(base.Name + ": Restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        break;
                    case Global.RecoveryStream.E:
                        if (VaderConsulting.Helper.Properties.StreamEServiceName.ToUpper() != "[NONE]")
                        {
                            Task = new RecoveryTask(base.Name + ": Use " + VaderConsulting.Helper.Properties.StreamEServiceName + " to restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        else
                        {
                            Task = new RecoveryTask(base.Name + ": Restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        }
                        break;
                    case Global.RecoveryStream.None3:
                        Task = new RecoveryTask(base.Name + ": Restore " + Disk.Name + " (" + Disk.TotalAllocatedGB + "GB)", "Perform Disk restore steps", "", Disk.RestoreTime, this);
                        break;
                    default:
                        break;
                }

                if (Task != null)
                {
                    _RecoveryTasks.Add(Task);
                }
                TotalTime += Disk.RestoreTime; // This time already includes any interval period for each disk
            }

            if (Task != null)
            {
                // Sort these tasks
                _RecoveryTasks = _RecoveryTasks.OrderBy(RecoveryTask => RecoveryTask.Name).ToList();
            }
            else
            {
                // No Tasks as the Stream is 'None' (i.e. X)
            }
            //Task = new RecoveryTask(base.Name + ": Perform Server restore steps (consult ARR/IRR)", "", "These steps are necessary to prepare the Server for use after recovery (See Application/Infrastructure Recovery Runbook)", new TimeSpan(0), this);

            //_RecoveryTasks.Add(Task);

            _RecoveryTime = TotalTime;

        }

        public List<string> GetShareNames()
        {
            List<string> Names = new List<string>();

            string Path = string.Format(@"\\{0}\root\cimv2", base.Name);
            string Query = "select * from win32_share";

            try
            {
                ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Path, Query);

                var Shares = Searcher.Get();

                foreach (ManagementObject Share in Shares)
                {
                    Names.Add(Share["Name"].ToString());
                }
            }
            catch (Exception)
            {
            }

            return Names;
        }

        public void RemoveRelationship(Relationship Relationship)
        {
            if (_Relationships.Contains(Relationship))
            {
                //_ServicesProvided.Remove(Relationship.Service);
                _ProvidedBusinessApplications.Remove(Relationship.FromBusinessApplication);
                _Relationships.Remove(Relationship);
            }
        }

        public void Save(VaderConsulting.Database.SQLServer SQL)
        {
            _SQL = SQL;
            Save();
        }

        public void Save()
        {
            //string Query = "";

            // TODO:  Get attribute ID's and bring them back to configurable options

            // Runbook Order
            //Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + base.RunbookOrder.ToString() + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'E94D58B7-8942-4A05-92EA-CD5942C49C07' AND VersionId = '" + base.VersionID.ToString() + "'";
            //_SQL.ExecuteNonQuery(Query);

            // In Scope
            //Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + Convert.ToInt32(_InScope) + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = 'D29C2600-F53E-4B25-8F4A-818FC7656FAE' AND VersionId = '" + base.VersionID.ToString() + "'";
            //_SQL.ExecuteNonQuery(Query);

            // Virtual
            //Query = "UPDATE AttributeValueBigInt SET AttributeValue = " + Convert.ToInt32(_Virtual) + " WHERE ObjectId = '" + base.ID.ToString() + "' AND AttributeId = '74CB39F9-FC2D-495C-B752-50031FF803B1' AND VersionId = '" + base.VersionID.ToString() + "'";
            //_SQL.ExecuteNonQuery(Query);

        }

        #endregion

        #region Overrides

        //public override void OnSystemStateChanged(object Sender, StateChangeEventArgs e)
        //{
        //    if (e.OldState == e.NewState)
        //        return;

        //    //base.OnStateChanged(Sender, e);

        //    Debug.WriteLine("Server state changed to " + Enum.GetName(typeof(Global.HealthState), e.NewState));

        //    foreach (BusinessApplication Service in this._DependentBusinessApplications)
        //    {
        //        if (base.SystemState > Service.SystemState)
        //        {
        //            Service.SetSystemHealthState(base.SystemState, base.Name + " state is " + Enum.GetName(typeof(Global.HealthState), e.NewState));
        //        }
        //    }
        //}

        public override void RaiseNewSystemState(Global.HealthState NewState, Global.HealthState OldState, string Name, string Text)
        {
            if (NewState != OldState)
            {
                base.RaiseNewSystemState(NewState, OldState, Name, Text);

                if (VaderConsulting.Helper.Properties.ShowDebugInformation)
                {
                    Debug.WriteLine(base.Name + " has changed state from " + OldState + " to " + NewState);
                }

                foreach (BusinessApplication Service in _DependentBusinessApplications)
                {
                    Service.CalculateState("");
                }
            }
        }

        public override string ToString()
        {
            return base.Name;
        }

        #endregion

    }
}
