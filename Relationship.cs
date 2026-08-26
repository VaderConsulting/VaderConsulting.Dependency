using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VaderConsulting.Dependency;

namespace VaderConsulting.Dependency
{
    public class Relationship : ComponentBase
    {
        #region Constructors

        public Relationship()
            : base()
        {
        }

        public Relationship(BusinessApplication BusinessApplication, BusinessApplication ComponentBusinessApplication)
            : base()
        {
            _FromBusinessApplication = BusinessApplication;
            _ToBusinessApplication = ComponentBusinessApplication;
            _ToType = ComponentType.BusinessApplication;

            if (Global.GetExistingBusinessApplication(ComponentBusinessApplication, BusinessApplication.ComponentBusinessApplications) == null)
            {
                BusinessApplication.ComponentBusinessApplications.Add(ComponentBusinessApplication);
                ComponentBusinessApplication.DependentBusinessApplications.Add(BusinessApplication);

                ComponentBusinessApplication.SystemStateChangeHandler += new SystemStateChangeEventHandler(ComponentBusinessApplication_SystemStateChangeHandler);
            }
        }

        public Relationship(ref BusinessApplication BusinessApplication, ref Server ComponentServer)
            : base()
        {
            _FromBusinessApplication = BusinessApplication;
            _ToServer = ComponentServer;
            _ToType = ComponentType.Server;

            if (Global.GetExistingServer(ComponentServer, BusinessApplication.ComponentServers) == null)
            {
                ComponentServer.DependentBusinessApplications.Add(BusinessApplication);
                ComponentServer.AddRelationship(this, ComponentServer.Relationships);

                ComponentServer.SystemStateChangeHandler += new SystemStateChangeEventHandler(ComponentServer_SystemStateChangeHandler);
            }
        }

        void ComponentServer_SystemStateChangeHandler(object Sender, StateChangeEventArgs e)
        {
            //Debug.WriteLine(e.Name + " (Server) has changed state from " + e.PreviousState + " to " + e.State + ": " + e.Text);
        }

        void ComponentBusinessApplication_SystemStateChangeHandler(object Sender, StateChangeEventArgs e)
        {
            //Debug.WriteLine(e.Name + " (Business Application) has changed state from " + e.PreviousState + " to " + e.State + ": " + e.Text);
        }

        public Relationship(string Name)
            : base()
        {
            base.Name = Name;
        }

        public Relationship(string Name, Global.HealthState State)
            : base()
        {
            base.Name = Name;
            //base.State = State;
            base.SetSystemHealthState(State, "");
        }

#endregion

        #region Enums

        public enum ComponentType : int
        {
            Server = 0,
            BusinessApplication = 1,
            Service = 3
        }

#endregion

        #region Fields

        private BusinessApplication _FromBusinessApplication = null;
        private Guid _FromObjectID = Guid.Empty;
        private string _Name = "";
        private Global.RecoveryStream _ResultantStream = Global.RecoveryStream.Undefined;
        private bool _StreamDependency = false;
        private bool _SystemMandatory = true;
        private BusinessApplication _ToBusinessApplication = null;
        private Guid _ToObjectID = Guid.Empty;
        private Server _ToServer = null;
        private Service _ToService = null;
        private ComponentType _ToType = ComponentType.Server;
        private bool _UserMandatory = false;

#endregion

        #region Overrides

        //public override void OnNewSystemState(object Sender, StateChangeEventArgs e)
        //{
        //    //base.OnStateChanged(Sender, e);

        //    Debug.WriteLine("Relationship state changed to " + Enum.GetName(typeof(Global.HealthState), e.NewState));
        //}

        public override string ToString()
        {
            return _Name;
        }

#endregion

        #region Properties

        /// <summary>
        /// This is the Service that has "focus"
        /// </summary>
        public BusinessApplication FromBusinessApplication
        {
            get
            {
                return _FromBusinessApplication;
            }
            set
            {
                _FromBusinessApplication = value;
            }
        }

        public Guid FromObjectID
        {
            get
            {
                return _FromObjectID;
            }
            set
            {
                _FromObjectID = value;
            }
        }

        public new string Name
        {
            get
            {
                if (_ToType == ComponentType.BusinessApplication)
                    if (_SystemMandatory)
                    {
                        _Name = _FromBusinessApplication.Name + " depends upon " + _ToBusinessApplication.Name;
                    }
                    else
                    {
                        _Name = _FromBusinessApplication.Name + " optionally depends upon " + _ToBusinessApplication.Name;
                    }
                else
                    if (_SystemMandatory)
                    {
                        _Name = _FromBusinessApplication.Name + " depends upon " + _ToServer.Name;
                    }
                    else
                    {
                        _Name = _FromBusinessApplication.Name + " optionally depends upon " + _ToServer.Name;
                    }

                return _Name;
            }
        }

        public Global.RecoveryStream ResultantStream
        {
            get
            {
                return _ResultantStream;
            }
            set
            {
                _ResultantStream = value;
            }
        }

        public bool StreamDependency
        {
            get
            {
                return _StreamDependency;
            }
            set
            {
                _StreamDependency = value;
            }
        }

        public bool SystemMandatory
        {
            get
            {
                return _SystemMandatory;
            }
            set
            {
                _SystemMandatory = value;
            }
        }

        /// <summary>
        /// This is the Service that is relied upon
        /// </summary>
        public BusinessApplication ToBusinessApplication
        {
            get
            {
                return _ToBusinessApplication;
            }
            set
            {
                if (value == null)
                {
                    _ToBusinessApplication = null;
                }
                else
                {
                    if (_ToBusinessApplication != null)
                    {
                        throw new Exception("You already have a Business Application dependency for this relationship");
                    }
                    else
                    {
                        _ToBusinessApplication = value;
                        _ToType = ComponentType.BusinessApplication;
                    }
                }
            }
        }

        public Guid ToObjectID
        {
            get
            {
                return _ToObjectID;
            }
            set
            {
                _ToObjectID = value;
            }
        }

        /// <summary>
        /// This is the Server that is relied upon
        /// </summary>
        public Server ToServer
        {
            get
            {
                return _ToServer;
            }
            set
            {
                if (value == null)
                {
                    _ToServer = null;
                }
                else
                {
                    if (_ToServer != null)
                    {
                        throw new Exception("You already have a Server dependency for this relationship");
                    }
                    else
                    {
                        _ToServer = value;
                        _ToType = ComponentType.Server;
                    }
                }
            }
        }

        /// <summary>
        /// Describes the connection type (Server or Service)
        /// </summary>
        public ComponentType ToType
        {
            get
            {
                return _ToType;
            }
        }

        public bool UserMandatory
        {
            get
            {
                return _UserMandatory;
            }
            set
            {
                _UserMandatory = value;
            }
        }

#endregion

        public Relationship Join(ref BusinessApplication FromBusinessApplication, ref BusinessApplication ToBusinessApplication)
        {
            _FromBusinessApplication = FromBusinessApplication;
            _ToBusinessApplication = ToBusinessApplication;
            _ToType = ComponentType.BusinessApplication;

            //_FromBusinessApplication.AddRelationship(this, _FromBusinessApplication.Relationships);
            _FromBusinessApplication.AddRelationship(this);
            //_ToBusinessApplication.AddRelationship(this, _ToBusinessApplication.Relationships);
            _ToBusinessApplication.AddRelationship(this);

            return this;
        }

        public Relationship Join(ref BusinessApplication FromBusinessApplication, ref Server ToServer)
        {
            _FromBusinessApplication = FromBusinessApplication;
            _ToServer = ToServer;
            _ToType = ComponentType.Server;

            //_FromBusinessApplication.AddRelationship(this, _FromBusinessApplication.Relationships);
            _FromBusinessApplication.AddRelationship(this);
            _ToServer.AddRelationship(this, _ToServer.Relationships);

            return this;
        }
    }
}
