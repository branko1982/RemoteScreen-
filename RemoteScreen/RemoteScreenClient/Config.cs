namespace RemoteScreenTransmiter
{
    static class Config
    {
        public static bool UseLog = true;
        public static int WaitTimeBeforeSendingImages = 1;
        public static string RemoteServerAddress = "";
        public static int RemoteServerPort = 0;
        public static int ScreenMaxWidth = 1366;
        public static int ScreenMaxHeight = 768;
        public static bool RunningProgramAllowed = true;
        public static bool SendingScreenOutputActive = false;
    }
}