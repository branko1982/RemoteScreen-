using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteScreenServer
{
    class SocketEntryManager
    {

        private static List<SocketEntry> socketEntries;
        public static List<SocketEntry> GetSocketEntries()
        {
            if(SocketEntryManager.socketEntries == null)
            {
                SocketEntryManager.socketEntries = new List<SocketEntry>();
            }
            return SocketEntryManager.socketEntries;
        }


    }
}
