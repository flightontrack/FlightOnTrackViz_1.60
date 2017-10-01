using System;
using System.IO;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track
{
    public class LogHelper
    {
        public LogHelper()
        {
        }

        public static void onLog(string source, string text)
        {
            try
            {
                if (!Directory.Exists(LOG_FILES_PATH)) Directory.CreateDirectory(LOG_FILES_PATH);
                using (FileStream filestream = new FileStream(LOG_FILES_PATH + "Consoleout.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamwriter = new StreamWriter(filestream);
                    Console.SetOut(streamwriter);
                    Console.SetError(streamwriter);
                    //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")+"|Error source: "+source+"|" + e);
                    Console.WriteLine("\n");
                    Console.WriteLine("*********************************************************************");
                    Console.WriteLine(DateTime.Now.ToString("u"));
                    Console.WriteLine("Log source: " + source + "|" + text);
                    Console.WriteLine("---------------------------------------------------------------------");
                    streamwriter.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void onFailureLog(string source, Exception e, string parname = "Unknown")
        {
            try
            {
                if (!Directory.Exists(LOG_FILES_PATH)) Directory.CreateDirectory(LOG_FILES_PATH);
                using (FileStream filestream = new FileStream(LOG_FILES_PATH + "ConsoleErrorOut.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamwriter = new StreamWriter(filestream);
                    Console.SetOut(streamwriter);
                    Console.SetError(streamwriter);
                    //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")+"|Error source: "+source+"|" + e);
                    Console.WriteLine("\n");
                    Console.WriteLine("*********************************************************************");
                    Console.WriteLine(DateTime.Now.ToString("u"));
                    Console.WriteLine("Error source: " + source + "|" + parname + "|" + e);
                    Console.WriteLine("---------------------------------------------------------------------");
                    streamwriter.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void onFailureLog(string source, string error)
        {
            try
            {
                if (!Directory.Exists(LOG_FILES_PATH)) Directory.CreateDirectory(LOG_FILES_PATH);
                using (FileStream filestream = new FileStream(LOG_FILES_PATH + "ConsoleErrorOut.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamwriter = new StreamWriter(filestream);
                    Console.SetOut(streamwriter);
                    Console.SetError(streamwriter);
                    //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")+"|Error source: "+source+"|" + e);
                    Console.WriteLine("\n");
                    Console.WriteLine("*********************************************************************");
                    Console.WriteLine(DateTime.Now.ToString("u"));
                    Console.WriteLine("Error source: " + source + "|" + error);
                    Console.WriteLine("---------------------------------------------------------------------");
                    streamwriter.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}