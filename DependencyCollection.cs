using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DC
{
    public class DependencyCollection
    {
        private List<Service> _Services = new List<Service>();
        private List<Server> _Servers = new List<Server>();

        public List<Service> Services
        {
            get
            {
                return _Services;
            }
        }

        public List<Server> Servers
        {
            get
            {
                return _Servers;
            }
        }

        public void AddServer(string Server)
        {
            // Create a new Server object and then call AddServer(Server Server, null)
        }

        public void AddServer(Server Server)
        {
            AddServer(Server, null);
        }

        public void AddServer(Server Server, Service Service)
        {
            //Console.WriteLine("---->Server count: " + _Servers.Count());
            
            // Look at any servers within the service and
            
            if (!_Servers.Exists(x => x.Name == Server.Name))
            {
                // 1. If the server doesn't exist add it
                _Servers.Add(Server);
            }
            else
            {
                // 2. If the server exists, add this service to it's list of provided services
                if (Service != null)
                {
                    Server ExistingServer = _Servers.Find(x => x.Name == Server.Name);

                    // Does the existing Server already have this Service in it's ServicesProvided list?
                    Service ExistingService = ExistingServer.ServicesProvided.Find(x => x.Name == Service.Name);

                    if (ExistingService == null)
                    {
                        // Could not find this service, so add it.
                        Server.ServicesProvided.Add(Service);
                    }

                }
            }
        }

        public void AddService(Service Service)
        {
            //Console.WriteLine("---->Service count: " + _Services.Count());
            
            // Add the service to the internal list
            //Console.WriteLine("---->Adding Service to List: " + Service.Name);
            _Services.Add(Service);

            foreach (Server Server in Service.Servers)
            {
                AddServer(Server, Service);
            }
        }

        public void AddService(string Service)
        {
            // Create a new Service and then call AddService(Service Service)
        }

        public void RemoveServer(string Server)
        {
            // Create a new Server object and then call RemoveServer(Server Server)
        }

        public void RemoveServer(Server Server)
        {
            // Check for the Server.  It it exists, remove it.
            // Then remove it from all Services
        }

        public void RemoveService(string Service)
        {
            // Create a new Service object and then call RemoveService(Service Service)
        }

        public void RemoveService(Service Service)
        {
            // Check for the Service.  It it exists, remove it.
        }

        public void RemoveServices()
        {
            _Services.Clear();
        }

        public void RemoveServers()
        {
            _Servers.Clear();

            foreach (Service Service in _Services)
            {
                Service.Servers.Clear();
            }
        }
    }
}
