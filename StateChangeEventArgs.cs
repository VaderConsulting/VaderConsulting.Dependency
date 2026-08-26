using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class StateChangeEventArgs : EventArgs
    {
        private string _Text = "";
        private DateTime _Time = DateTime.Now;
        private Global.HealthState _State = Global.HealthState.Unknown;
        private Global.HealthState _PreviousState = Global.HealthState.Unknown;
        private string _Name = "";

        public StateChangeEventArgs(Global.HealthState NewState, Global.HealthState OldState, string Name, string Text)
        {
            _Text = Text;
            _PreviousState = _State;
            _State = NewState;
            _Name = Name;
        }

        public string Text
        {
            get
            {
                return _Text;
            }
        }

        public Global.HealthState State
        {
            get
            {
                return _State;
            }
        }

        public Global.HealthState PreviousState
        {
            get
            {
                return _PreviousState;
            }
        }

        public DateTime Time
        {
            get
            {
                return _Time;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }
    }
}
