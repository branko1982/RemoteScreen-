using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using TinyJSON;

namespace RemoteScreenOperator
{
    class FormVideoChecker
    {
        private List<FormVideo> formVideos;
        private Logger logger;
        private Socket socket;
        public FormVideoChecker(List<FormVideo> formVideos, Logger logger, Socket socket)
        {
            this.formVideos = formVideos;
            this.logger = logger;
            this.socket = socket;
        }
        public void Main()
        {
            //logger.Log("FormVideoChecker thread started");
            try
            {
                while(true)
                {
                    foreach(FormVideo formVideo in formVideos)
                    {
                        if(!formVideo.IsFormOpen)
                        {
                            // logger.Log("form Video: " + formVideo.clientInfo.deviceName + "will be removed");
                            formVideos.Remove(formVideo); //dúfam že nájde správny formVideo

                            var clients = new List<ClientInfo>();
                            clients.Add(formVideo.clientInfo);
                               
                            DataJsonObjectTemplate dataJsonObjectTemplate = new DataJsonObjectTemplate();
                            dataJsonObjectTemplate.dataType = "tell_client_to_stop_streaming_vid";
                            dataJsonObjectTemplate.clients = clients;


                            byte[] json = Encoding.UTF8.GetBytes(JSON.Dump(dataJsonObjectTemplate, EncodeOptions.NoTypeHints));

                            socket.Send(json);

                            //na server sa pošle informácia že formVideo nieje otvorený a teda sa má zastaviť posielanie video výstupu
                            //tiež, video výstup čo sa mne už nestihol odoslať by mal byť z klienta zmazaný
                            break;
                        }
                    }
                    Thread.Sleep(500); //vlákno bude spať jednu sekundu. CPU by sa tak nemal zblázniť
                }

            }
            catch (Exception e)
            {
                logger.Log("[FormVideoChecker].cs exception: " + e.Message);
            }
        }
    }
}
