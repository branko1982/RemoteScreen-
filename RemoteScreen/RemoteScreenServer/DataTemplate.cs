using System.Collections.Generic;
using TinyJSON;
namespace RemoteScreenServer
{
    class DataJsonObjectTemplate
    {
        /*pomenovácia konvencia ktorú microsoft požaduje pri premenných je tu porušená, pretožer názvy premenných v tomto objekte budú názvy kľúčov v json stringu. Kde je podľa
         mňa vhodnejšie použiť podtržítka na oddelovanie slov*/
        [Include]
        public string dataType;

        [Include]
        public List<ClientInfo> clients;

    }
    class ClientInfo
    {
        [Include]
        public string ipAddress;
        [Include]
        public int portNumber;
        [Include]
        public string deviceName;
    }
}
