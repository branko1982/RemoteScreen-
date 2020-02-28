using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteScreenReceiver
{
    class ConnectedClientListBoxItem
    {
        public string IpAddress { get; set; }
        public int Id { get; set; }

        public ConnectedClientListBoxItem(string ipAddress, int id)
        {
            this.IpAddress = ipAddress;
            this.Id = id;
        }
    }
}
