using System;
using System.Windows.Forms;
using System.Threading;
using RemoteScreenOperator;
class Logger
{
    public Logger(RichTextBox logContainer)
    {
        this.logContainer = logContainer;
    }
    private RichTextBox logContainer;

    public void setLogContainer(RichTextBox logContainer)
    {
        this.logContainer = logContainer;
    }
    public void Log(string input)
    {
        if (Config.UseLog)
        {
            if (logContainer != null)
            {
                try
                {
                    string toAppend = DateTime.Now.ToString() + " : " + input + "\n";
                    logContainer.Invoke(new MethodInvoker(delegate { logContainer.Text += toAppend; }));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
