using System.IO;
using System.Reflection;
using System;
using System.Text;

namespace VinnyLibConverterUtils
{
    public class VinnyLibConverterLogger
    {
        public enum MessageType
        {
            Info,
            Error,
            Warning,
        }
        public VinnyLibConverterLogger()
        {
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string execution_directory_path = Path.GetDirectoryName(executingAssemblyFile);

            string logDirPath = Path.Combine(execution_directory_path, "logs");
            Directory.CreateDirectory(logDirPath);
            pLogPath = Path.Combine(logDirPath, DateTime.Now.ToString("u"));

            mLog = new StringBuilder();

        }
        public static VinnyLibConverterLogger InitLogger()
        {
            if (mInstance == null) mInstance = new VinnyLibConverterLogger();
            return mInstance;
        }

        public void WriteLog(string message, MessageType mType = MessageType.Info, bool directToFile = false)
        {
            string timeStart = DateTime.Now.ToString("T");
            string mesPrefix = mType.ToString();
            string log = timeStart + "\t" + mesPrefix + "\t" + message;
            
            mLog.AppendLine(log);
            if (directToFile) SaveLog();

            if (mLog.Length > 10000) SaveLog();
        }

        public void SaveLog()
        {
            File.AppendAllText(pLogPath, mLog.ToString());
            mLog.Clear();
        }

        private StringBuilder mLog; 
        private string pLogPath = "";
        private static VinnyLibConverterLogger mInstance;
    }
}
