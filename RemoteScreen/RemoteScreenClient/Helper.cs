using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteScreenTransmiter
{
    class Helper
    {
        public void printByteArray(byte[] array) {
            for(int x = 0; x < array.Length; x++)
            {
                System.Diagnostics.Debug.WriteLine("[Helper.printByteArray] value in index: "+ x  +" is " + array[x]);
            }
        }
    }
}
