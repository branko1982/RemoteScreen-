using System;

namespace RemoteScreenServer
{
    class Logger
    {
        public static void Log(string input)
        {

            if (true)
            {
                try
                {
                    string toAppend = "[" + DateTime.Now.ToString() + "]" + input;
                    Console.WriteLine(toAppend);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

