using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class ComponentBase : IDisposable
    {
        #region Fields

        private string _Name = "";
        private Guid _ID = Guid.Empty;
        private Guid _VersionID = new Guid();
        private bool _Flag = false;
        //private Int32 _RunbookOrder = -1;

        private bool _RaiseSystemStateChangeEventFromUnknown = false;
        private bool _RaiseUserStateChangeEventFromUnknown = false;
        
        private Global.HealthState _ActualSystemState = Global.HealthState.OK;
        private Global.HealthState _EmulatedSystemState = Global.HealthState.OK;
        private string _ActualSystemStateDescription = "OK";
        private string _EmulatedSystemStateDescription = "OK";
        private bool _SystemStateIsEmulated = false;

        private Global.HealthState _ActualUserState = Global.HealthState.OK;
        private Global.HealthState _EmulatedUserState = Global.HealthState.OK;
        private string _ActualUserStateDescription = "OK";
        private string _EmulatedUserStateDescription = "OK";
        private bool _UserStateIsEmulated = false;

        private List<string> _Warnings = new List<string>();

        #endregion

        public delegate void SystemStateChangeEventHandler(object Sender, VaderConsulting.Dependency.StateChangeEventArgs e);
        public event SystemStateChangeEventHandler SystemStateChangeHandler;

        public delegate void UserStateChangeEventHandler(object Sender, VaderConsulting.Dependency.StateChangeEventArgs e);
        public event UserStateChangeEventHandler UserStateChangeHandler;

        #region Constructors

        public ComponentBase()
        {
            DoStartup(null, Global.HealthState.Unknown, Global.HealthState.Unknown, false, false);
        }

        public ComponentBase(string Name)
        {
            DoStartup(Name, Global.HealthState.Unknown, Global.HealthState.Unknown, false, false);
        }

        public ComponentBase(string Name, Global.HealthState SystemState, Global.HealthState UserState)
        {
            DoStartup(Name, SystemState, UserState, false, false);
        }

        public ComponentBase(string Name, Global.HealthState SystemState, Global.HealthState UserState, bool RaiseSystemStateChangeEventFromUnknown, bool RaiseUserStateChangeEventFromUnknown)
        {
            DoStartup(Name, SystemState, UserState, RaiseSystemStateChangeEventFromUnknown, RaiseUserStateChangeEventFromUnknown);
        }

        #endregion

        #region Destructor

        ~ComponentBase()
        {
            DoDisposal();
        }

        private void DoDisposal()
        {
            //Debug.WriteLine("GC: [" + _Name + " was removed]");
        }

        #endregion

        #region Properties

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

        public Guid ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public Global.HealthState SystemState
        {
            get
            {
                if (_SystemStateIsEmulated)
                {
                    return _EmulatedSystemState;
                }
                else
                {
                    return _ActualSystemState;
                }
            }
        }

        public Global.HealthState UserState
        {
            get
            {
                if (_UserStateIsEmulated)
                {
                    return _EmulatedUserState;
                }
                else
                {
                    return _ActualUserState;
                }
            }
        }

        public bool RaiseSystemStateChangeEventFromUnknown
        {
            get
            {
                return _RaiseSystemStateChangeEventFromUnknown;
            }
            set
            {
                _RaiseSystemStateChangeEventFromUnknown = value;
            }
        }

        public bool RaiseUserStateChangeEventFromUnknown
        {
            get
            {
                return _RaiseUserStateChangeEventFromUnknown;
            }
            set
            {
                _RaiseUserStateChangeEventFromUnknown = value;
            }
        }

        public string SystemStateDescription
        {
            get
            {
                if (_SystemStateIsEmulated)
                {
                    return _EmulatedSystemStateDescription;
                }
                else
                {
                    return _ActualSystemStateDescription;
                }
            }
            set
            {
                if (_SystemStateIsEmulated)
                {
                    _EmulatedSystemStateDescription = value;
                }
                else
                {
                    _ActualSystemStateDescription = value;
                }
                
            }
        }

        public string UserStateDescription
        {
            get
            {
                if (_UserStateIsEmulated)
                {
                    return _EmulatedUserStateDescription;
                }
                else
                {
                    return _ActualUserStateDescription;
                }
            }
            set
            {
                if (_UserStateIsEmulated)
                {
                    _EmulatedUserStateDescription = value;
                }
                else
                {
                    _ActualUserStateDescription = value;
                }

            }
        }

        public bool Flag
        {
            get
            {
                return _Flag;
            }
            set
            {
                _Flag = value;
            }
        }

        public bool SystemStateIsEmulated
        {
            get
            {
                return _SystemStateIsEmulated;
            }
            set
            {
                _SystemStateIsEmulated = value;
            }
        }

        public bool UserStateIsEmulated
        {
            get
            {
                return _UserStateIsEmulated;
            }
            set
            {
                _UserStateIsEmulated = value;
            }
        }

        //public Int32 RunbookOrder
        //{
        //    get
        //    {
        //        return _RunbookOrder;
        //    }
        //    set
        //    {
        //        _RunbookOrder = value;
        //    }
        //}

        public Guid VersionID
        {
            get
            {
                return _VersionID;
            }
            set
            {
                _VersionID = value;
            }
        }

        public List<string> Warnings
        {
            get
            {
                return _Warnings;
            }

            set
            {
                _Warnings = value;
            }
        }

        #endregion

        #region Public Methods

        public virtual void SetSystemHealthState(Global.HealthState NewSystemState, string Reason)
        {
            if (_SystemStateIsEmulated)
            {
                Global.HealthState OldState = _EmulatedSystemState;

                _EmulatedSystemState = NewSystemState;
                _EmulatedSystemStateDescription = Reason;

                if ((_RaiseSystemStateChangeEventFromUnknown && OldState == Global.HealthState.Unknown) || OldState != Global.HealthState.Unknown)
                {
                    RaiseNewSystemState(NewSystemState, OldState, this.Name, Reason); // <-- Raise new event for the state change
                }
            }
            else
            {
                Global.HealthState OldState = _ActualSystemState;

                _ActualSystemState = NewSystemState;
                _ActualSystemStateDescription = Reason;

                if ((_RaiseSystemStateChangeEventFromUnknown && OldState == Global.HealthState.Unknown) || OldState != Global.HealthState.Unknown)
                {
                    RaiseNewSystemState(NewSystemState, OldState, this.Name, Reason); // <-- Raise new event for the state change
                }
            }
        }

        public virtual void SetUserHealthState(Global.HealthState NewUserState, string Reason)
        {
            if (_UserStateIsEmulated)
            {
                Global.HealthState OldState = _EmulatedUserState;
                
                _EmulatedUserState = NewUserState;
                _EmulatedUserStateDescription = Reason;

                if ((_RaiseSystemStateChangeEventFromUnknown && OldState == Global.HealthState.Unknown) || OldState != Global.HealthState.Unknown)
                {
                    RaiseNewSystemState(NewUserState, OldState, this.Name, Reason); // <-- Raise new event for the state change
                }
            }
            else
            {
                Global.HealthState OldState = _ActualUserState;
                
                _ActualUserState = NewUserState;
                _ActualUserStateDescription = Reason;

                if ((_RaiseSystemStateChangeEventFromUnknown && OldState == Global.HealthState.Unknown) || OldState != Global.HealthState.Unknown)
                {
                    RaiseNewSystemState(NewUserState, OldState, this.Name, Reason); // <-- Raise new event for the state change
                }
            }
        }

        #endregion

        #region Helper Methods

        //private void AddEventHandlers()
        //{
        //    StateChanged += new StateChangeEventHandler(OnStateChanged);
        //}

        //private void PerformStateChangedActions(object Sender, StateChangeEventArgs e)
        //{
        //    if (e.OldState != e.NewState)
        //    {
        //        Debug.WriteLine(_Name + " State change!  New State is " + Enum.GetName(typeof(Global.HealthState), e.NewState));
        //    }
        //}

        private void DoStartup(string Name, Global.HealthState SystemState, Global.HealthState UserState, bool RaiseSystemStateChangeEventFromUnknown, bool RaiseUserStateChangeEventFromUnknown)
        {
            _Name = Name;
            _ID = Guid.NewGuid();

            _RaiseSystemStateChangeEventFromUnknown = RaiseSystemStateChangeEventFromUnknown;
            _ActualSystemState = SystemState;

            _RaiseUserStateChangeEventFromUnknown = RaiseUserStateChangeEventFromUnknown;
            _ActualUserState = UserState;
        }

        #endregion

        #region Event creation methods

        public virtual void RaiseNewSystemState(Global.HealthState NewState, Global.HealthState OldState, string Name, string Text) // Call this method to raise the 'NewState' Event
        {
            SystemStateChangeEventHandler Raiser = SystemStateChangeHandler;

            VaderConsulting.Dependency.StateChangeEventArgs e = new VaderConsulting.Dependency.StateChangeEventArgs(NewState, OldState, Name, Text);

            if (Raiser != null)
            {
                Raiser(this, e);
            }
        }

        public virtual void RaiseNewUserState(Global.HealthState NewState, Global.HealthState OldState, string Name, string Text) // Call this method to raise the 'NewState' Event
        {
            UserStateChangeEventHandler Raiser = UserStateChangeHandler;

            VaderConsulting.Dependency.StateChangeEventArgs e = new VaderConsulting.Dependency.StateChangeEventArgs(NewState, OldState, Name, Text);

            if (Raiser != null)
            {
                Raiser(this, e);
            }
        }

        #endregion

        public virtual new string ToString()
        {
            return _Name;
        }

        public virtual void Dispose()
        {
            DoDisposal();
        }

        

    }
}
