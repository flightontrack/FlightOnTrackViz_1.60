using System;
using System.Diagnostics;
using System.IO;
using static FontNameSpace.Finals;

namespace FontNameSpace
{
    public class LogHelper
    {
        public LogHelper()
        {
        }
        public async void onLog(string source, string text)
        {
            var filePathLog = LOG_FILES_PATH + "ConsoleOut.txt";
            FileStream filestream = null;
            try
            {
                Directory.CreateDirectory(LOG_FILES_PATH);
                filestream = new FileStream(filePathLog, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                //{
                    string l = "\n";
                    //l += "---------------------------------------------------------\n";
                    l += (DateTime.Now.ToString("u")) + "\n";
                    l += "Log source: " + source + "|" + text + "\n";
                    l += "---------------------------------------------------------\n";
                    using (StreamWriter streamwriter = new StreamWriter(filestream))
                    {
                        Debug.Print("Streaming >>>>>>>>>>>>>");
                        await streamwriter.WriteAsync(l);
                    }
                //}
            }
            catch (Exception ex)
            {
                Debug.Print("Dedug+++++++++++++" + ex.Message);
                Console.WriteLine("Console+++++++++++++" + ex.Message);
            }
            finally
            {
                if (filestream != null)
                    filestream.Dispose();
            }
        }

        public static void onExceptionLog(string source, Exception e, string parname = "Unknown")
        {
            var filePathEx = LOG_FILES_PATH + "FailureOut.txt";
            FileStream filestream = null;
            try
            {
                Directory.CreateDirectory(LOG_FILES_PATH);
                filestream = new FileStream(filePathEx, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                //{
                    string l = "\n";
                    //l += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
                    l += (DateTime.Now.ToString("u")) + "\n";
                    l += "Log source: " + source + "|" + e.Message + "\n";
                    l += "++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
                    using (StreamWriter streamwriter = new StreamWriter(filestream))
                    {
                        streamwriter.Write(l);
                    }
                //}
            }
            catch (Exception ex)
            {
                Debug.Print("Dedug ------ " + ex.Message);
                Console.WriteLine("Console ------- " + ex.Message);
            }
            finally
            {
                if (filestream != null)
                    filestream.Dispose();
            }
        }
    }
}