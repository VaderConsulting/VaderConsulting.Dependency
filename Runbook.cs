using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class Runbook
    {
        /// <summary>
        /// Instructs the algorithm where to place newly inserted records
        /// </summary>
        public enum InsertionMethod : int
        {
            /// <summary>
            /// Inserts the new record at the bottom of the list i.e. with the least priority
            /// </summary>
            Bottom = 0,
            /// <summary>
            /// Inserts the new record at the top of the list i.e. with the most priority
            /// </summary>
            Top = 1
        }
        
        #region Fields

        private List<Server> _Servers = new List<Server>();
        private List<BusinessApplication> _Services = new List<BusinessApplication>();
        private InsertionMethod _InsertMethod = InsertionMethod.Bottom;

        #endregion

        #region Constructors

        public Runbook()
        {

        }

        #endregion

        #region Properties

        public List<Server> Servers
        {
            get
            {
                return _Servers;
            }
        }

        public List<BusinessApplication> Services
        {
            get
            {
                return _Services;
            }
        }

        public InsertionMethod InsertMethod
        {
            get
            {
                return _InsertMethod;
            }
            set
            {
                _InsertMethod = value;
            }
        }

        #endregion

        #region Public Methods

        public void AddServer(Server Server, BusinessApplication ProvidedService)
        {
            if (_Servers.Count > 0)
            {
                // There is already 1 or more Servers, so work out where this one fits
                
            }
            else
            {
                _Servers.Add(Server);
            }
        }

        public void AddService(BusinessApplication Service)
        {
            if (_Services.Count > 0)
            {
                // There is already 1 or more Services, so work out where this one fits

            }
            else
            {
                foreach (Server Server in Service.ComponentServers)
                {
                    AddServer(Server, Service);
                }
                _Services.Add(Service);
            }
            
        }

        public void RemoveServer(Server Server)
        {
            if (_Servers.Count > 1)
            {

            }
            else
            {
                // There is only 1 Server, so just remove it.
                _Servers.Remove(Server);
            }
        }

        public void RemoveService(BusinessApplication Service)
        {
            foreach (Server Server in Service.ComponentServers)
            {
                RemoveServer(Server);
            }

            if (_Services.Count > 1)
            {

            }
            else
            {
                // There is only 1 Service, so just remove it.
                _Services.Remove(Service);
            }
        }

        #endregion

    }
}
