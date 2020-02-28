using System;
using System.Drawing;
using System.Windows.Forms;

namespace RemoteScreenOperator
{
    class FormVideo
    {
        public bool IsFormOpen { get; set; }
        public Logger logger { get; set; }

        public Form form;
        private PictureBox pictureBox1;

        public ClientInfo clientInfo;
        public FormVideo(ClientInfo clientInfo)
        {
            try
            { 
                IsFormOpen = true;
                form = new Form();
                form.Width = 800;
                form.Height = 470;
                form.Visible = true;

                this.clientInfo = clientInfo;

                form.Text = clientInfo.deviceName;
                pictureBox1 = new PictureBox();
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Width = 800;
                pictureBox1.Height = 450;
                form.Controls.Add(pictureBox1);
                form.Resize += OnFormResize;
                form.FormClosed += OnFormClosed;

            }
            catch (Exception ex)
            {
                logger.Log("exception in RemoteScreenReceiver::FormVideo[" + clientInfo.deviceName + "] Constructor -> " + ex.Message);
            }
        }
        public void UpdatePictureBox(Bitmap bitmap) {
            try
            {
                pictureBox1.Image = bitmap;
            }
            catch (Exception ex) {
                logger.Log("exception in RemoteScreenReceiver::FormVideo[" + clientInfo.deviceName + "].Form1_Resize() -> " + ex.Message);

            }
        }
        private void OnFormResize(object sender, System.EventArgs e)
        {
            try
            {
                Control control = (Control)sender;

                // Ensure the Form remains square (Height = Width).
                pictureBox1.Size = new Size(control.Size.Width - 50, control.Size.Height - 50);
            } catch(Exception ex) {
                logger.Log("exception in RemoteScreenReceiver::FormVideo[" + clientInfo.deviceName + "].Form1_Resize() -> " + ex.Message);
            }
        }
        private void OnFormClosed(object sender, System.EventArgs e)
        {
            logger.Log("[i] in RemoteScreenReceiver::FormVideo[" + clientInfo.deviceName+ "].OnFormClosed()");
            this.IsFormOpen = false;
        }

    }
}
