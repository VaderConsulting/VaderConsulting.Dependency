using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class RecoveryTask
    {
        private string _Name = "";
        private string _Description = "";
        private string _Comment = "";
        private TimeSpan _Time = new TimeSpan(0);
        private Server _Server = null;
        private bool _Completed = false;

        public RecoveryTask()
        {
        }

        public RecoveryTask(string Name)
        {
            _Name = Name;
        }

        public RecoveryTask(string Name, string Description)
        {
            _Name = Name;
            _Description = Description;
        }

        public RecoveryTask(string Name, string Description, string Comment)
        {
            _Name = Name;
            _Description = Description;
            _Comment = Comment;
        }

        public RecoveryTask(string Name, string Description, string Comment, TimeSpan Time)
        {
            _Name = Name;
            _Description = Description;
            _Comment = Comment;
            _Time = Time;
        }

        public RecoveryTask(string Name, string Description, string Comment, TimeSpan Time, Server Server)
        {
            _Name = Name;
            _Description = Description;
            _Comment = Comment;
            _Time = Time;
            _Server = Server;
        }

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

        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
            }
        }

        public TimeSpan Time
        {
            get
            {
                return _Time;
            }

            set
            {
                Time = value;
            }
        }

        public Server Server
        {
            get
            {
                return _Server;
            }

            set
            {
                _Server = value;
            }
        }

        public bool Completed
        {
            get
            {
                return _Completed;
            }

            set
            {
                _Completed = value;
            }
        }

    }
}
