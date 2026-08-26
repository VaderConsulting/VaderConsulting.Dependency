using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class Drawing
    {
        #region Fields

        private string _ID = "";
        private string _Name = "";
        private int _Version = 0;
        private Global.DrawingStatus _Status = Global.DrawingStatus.Not_Applicable;
        private string _Description = "";
        private Guid _VersionID = Guid.Empty;
        private string _LastUpdate = "";
        private string _Notes = "";
        private bool _InError = false;
        private System.Drawing.Image _Image = null;
        private VaderConsulting.Database.SQLServer _SQL = null;

        #endregion

        #region Properties

        public string ID
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

        public Global.DrawingStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case Global.DrawingStatus.Not_Applicable:
                        return "N/A";
                        //break;
                    case Global.DrawingStatus.Draft:
                        return "Draft";
                        //break;
                    case Global.DrawingStatus.Pending_Review:
                        return "Pending";
                        //break;
                    case Global.DrawingStatus.Approved:
                        return "Approved";
                        //break;
                    default: // else
                        return "N/A";
                        //break;
                }
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

        public string LastUpdate
        {
            get
            {
                return _LastUpdate;
            }
            set
            {
                _LastUpdate = value;
            }
        }

        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
            }
        }

        public bool InError
        {
            get
            {
                return _InError;
            }

            set
            {
                _InError = true;
            }
        }

        public System.Drawing.Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
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

        #endregion

        #region Constructors

        public Drawing()
        {

        }

        public Drawing(string ObjectID)
        {
            _ID = ObjectID;
        }

        public Drawing(string ObjectID, string Name)
        {
            _ID = ObjectID;
            _Name = Name;
        }

        public Drawing(string ObjectID, string Name, int Version)
        {
            _ID = ObjectID;
            _Name = Name;
            _Version = Version;
        }

        public Drawing(string ObjectID, string Name, int Version, Global.DrawingStatus Status)
        {
            _ID = ObjectID;
            _Name = Name;
            _Version = Version;
            _Status = Status;
        }

        #endregion

        #region Public methods

        public void Save(VaderConsulting.Database.SQLServer SQL)
        {
            _SQL = SQL;
            Save();
        }

        public void Save()
        {
            string Query = "";
            
            // TODO:  Get attribute ID's and bring them back to configurable options

            // Drawing Status
            Query = "UPDATE AttributeValueText SET AttributeValue = " + Convert.ToInt16(this.Status).ToString() + " WHERE AttributeId = 'C302BC0C-1FB1-4E25-A7AE-AC018FF27190' AND VersionId = '" + this.VersionID + "'";
            _SQL.ExecuteNonQuery(Query);

            // Drawing Last Update
            Query = "UPDATE AttributeValueText SET AttributeValue = '" + LastUpdate + "' WHERE AttributeId = 'E2A4F6ED-5764-46E5-B862-CE8982CD2C35' AND VersionId = '" + this.VersionID + "'";
            _SQL.ExecuteNonQuery(Query);

        }

        public override string ToString()
        {
            return _Name;
        }

        #endregion
    }
}
