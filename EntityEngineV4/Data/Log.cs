﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EntityEngineV4.Engine;

namespace EntityEngineV4.Data
{
    public sealed class Alert
    {
        public static readonly Alert Info = new Alert("Info");
        public static readonly Alert Warning = new Alert("Warning");
        public static readonly Alert Error = new Alert("Error");
        public static readonly Alert Critical = new Alert("Critical");

        public string Value { get; private set; }
        private Alert(string s)
        {
            Value = s;
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

        public Log()
        {
            LogLocation = "Log";
            if (!Directory.Exists(LogLocation))
            {
                Directory.CreateDirectory(LogLocation);
            }
            LogName = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                      DateTime.Now.Second + "-" + DateTime.Now.Millisecond + "_Log.Txt";

            LogLocation = Path.Combine(LogLocation,
                                        LogName);
            _file = new StreamWriter(LogLocation, true);
            _file.WriteLine("Starting Log @ " + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +
                            " | " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" +
                            DateTime.Now.Millisecond);
            _file.WriteLine("===========================================");
            _file.Close();
        }

        public Log(string logLocation)
        {
            LogLocation = logLocation;
            if (!Directory.Exists(logLocation))
            {
                Directory.CreateDirectory(LogLocation);
            }

            logLocation = Path.Combine(LogLocation,
                                       DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Minute + "-" +
                                       DateTime.Now.Millisecond + "_EntityEngineLog.Txt");

            _file = new StreamWriter(logLocation, true);
            _file.WriteLine("Starting Log @ " + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +
                            " | " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" +
                            DateTime.Now.Millisecond);
            _file.WriteLine("===========================================");
            _file.Close();
        }

        public void Write(string message, Alert l)
        {
            //Figure out what our Alert level is.
            string logline = "[" + DateTime.Now.Month + "-" + DateTime.Now.Day + " : " + DateTime.Now.Hour + ":" +
                             DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond +
                             //[Month-Day : HH:MM:SS:MS]
                             DateTime.Now.Millisecond + "]" + " - [" + l.Value + "]" +
                             " - " + message;

            _file = new StreamWriter(LogLocation, true);
            _file.WriteLine(logline);
            _file.Close();
        }

        public void Write(string message, IComponent sender, Alert l)
        {

            string logline =
                "[" + DateTime.Now.Month + "-" + DateTime.Now.Day + " : " + DateTime.Now.Hour + ":" +
                DateTime.Now.Minute + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond +
                //[Month-Day : HH:MM:SS:MS]
                DateTime.Now.Millisecond + "]" + " - [" + l.Value + "]" +
                " - [Sender: " + sender.Name + "] - " + message;
            _file = new StreamWriter(LogLocation, true);
            _file.WriteLine(logline);
            _file.Close();

            if (CheckLogSize())
            {
                
            }
        }

        public bool CheckLogSize()
        {
            long size = 0;
            FileInfo f = new FileInfo(LogLocation);
            size = f.Length;

            if (size > MAXLOGSIZE)
                return true;
            return false;
        }
}
}
