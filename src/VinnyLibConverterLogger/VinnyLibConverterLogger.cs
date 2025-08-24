using System.IO;
using System.Reflection;
using System;
using System.Text;

namespace VinnyLibConverterUtils
{
    public class VinnyLibConverterLogger
    {
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

        public void WriteLog(string message)
        {
            string timeStart = DateTime.Now.ToString("T");
            mLog.AppendLine(timeStart + "\t" + message);

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
