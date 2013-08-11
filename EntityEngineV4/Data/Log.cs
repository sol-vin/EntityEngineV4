using System;
using System.IO;
using EntityEngineV4.Engine;

namespace EntityEngineV4.Data
{
    public sealed class Alert
    {
        public static readonly Alert Trivial = new Alert("Trivial", 0);
        public static readonly Alert Info = new Alert("Info", 1);
        public static readonly Alert Warning = new Alert("Warning", 2);
        public static readonly Alert Error = new Alert("Error", 3);
        public static readonly Alert Critical = new Alert("Critical", 4);

        public string Value { get; private set; }

        public byte Rank { get; private set; }

        public static implicit operator string(Alert a)
        {
            return a.Value;
        }

        public Alert(string value, byte rank)
        {
            Value = value;
            Rank = rank;
        }
    }

    /// <summary>
    /// Logs information and messages based on inputs
    /// </summary>
    ///
    public class Log
    {
        public string LogLocation { get; private set; }

        private StreamWriter _file;

        public const double MAXLOGSIZE = 1024 * 1024 * 25;
        public string LogName;
        public readonly string FileExt = ".txt";

        public Alert HighestAlertLevel = Alert.Info;

        public int Id;

        public Log()
        {
            LogLocation = "Log";
            if (!Directory.Exists(LogLocation))
            {
                Directory.CreateDirectory(LogLocation);
            }
            LogName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                      DateTime.Now.Second + "-" + DateTime.Now.Millisecond + "_Log" + Id;

            LogLocation = System.IO.Path.Combine(LogLocation,
                                        LogName + FileExt);
            _file = new StreamWriter(LogLocation, true);
            _file.WriteLine("Starting Log @ " + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +
                            " | " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" +
                            DateTime.Now.Millisecond);
            _file.WriteLine("===========================================");
        }

        public Log(string logLocation)
        {
            LogLocation = logLocation;
            if (!Directory.Exists(logLocation))
            {
                Directory.CreateDirectory(LogLocation);
            }

            LogName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                      DateTime.Now.Second + "-" + DateTime.Now.Millisecond + "_Log" + Id;

            LogLocation = System.IO.Path.Combine(LogLocation,
                                        LogName + FileExt);

            _file = new StreamWriter(logLocation, true);
            _file.WriteLine("Starting Log @ " + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +
                            " | " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" +
                            DateTime.Now.Millisecond);
            _file.WriteLine("===========================================");
        }

        public void Write(string message, Alert l)
        {
            //Find out if our alert is not high enough to publish
            if (l.Rank < HighestAlertLevel.Rank)
                return;

            //Figure out what our Alert level is.
            string logline = "[" + DateTime.Now.Month + "-" + DateTime.Now.Day + " : " + DateTime.Now.Hour + ":" +
                             DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond +
                //[Month-Day : HH:MM:SS:MS]
                             DateTime.Now.Millisecond + "]" + " - [" + l + "]" +
                             " - " + message;
            if (CheckLogSize())
            {
                Id++;
                LogName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                          DateTime.Now.Second + "-" + DateTime.Now.Millisecond + "_Log" + Id;
            }
            _file.WriteLine(logline);
            _file.Flush();
        }

        public void Write(string message, IComponent sender, Alert l)
        {
            //Find out if our alert is not high enough to publish
            if (l.Rank < HighestAlertLevel.Rank)
                return;

            string sendersname;
            if (sender is Entity)
                sendersname = sender.Parent.Name + "->" + sender.Name;
            else if (sender is Component)
            {
                Component c = (Component)sender;
                sendersname = c.Parent.Name + "->" + c.Name;
            }
            else if (sender is Service)
            {
                sendersname = sender.Parent.Name + "->" + sender.Name;
            }
            else
            {
                sendersname = sender.Name;
            }
            string logline =
                "[" + DateTime.Now.Month + "-" + DateTime.Now.Day + " : " + DateTime.Now.Hour + ":" +
                DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond +
                //[Month-Day : HH:MM:SS:MS]
                DateTime.Now.Millisecond + "]" + " - [" + l + "]" +
                " - [Sender: " + sendersname + "] - " + message;

            if (CheckLogSize())
            {
                Id++;
                LogName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                          DateTime.Now.Second + "-" + DateTime.Now.Millisecond + "_Log" + Id;
            }

            _file.WriteLine(logline);
            _file.Flush();
        }

        public bool CheckLogSize()
        {
            long size = 0;
            var f = new FileInfo(LogLocation);
            size = f.Length;

            if (size < MAXLOGSIZE)
                return true;
            return false;
        }

        public void Dispose()
        {
            _file.Close();
        }
    }
}