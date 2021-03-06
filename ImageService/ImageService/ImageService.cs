﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Configuration;
using ImageService.Infrastructure;

public enum ServiceState
{
    SERVICE_STOPPED = 0x00000001,
    SERVICE_START_PENDING = 0x00000002,
    SERVICE_STOP_PENDING = 0x00000003,
    SERVICE_RUNNING = 0x00000004,
    SERVICE_CONTINUE_PENDING = 0x00000005,
    SERVICE_PAUSE_PENDING = 0x00000006,
    SERVICE_PAUSED = 0x00000007,
}

[StructLayout(LayoutKind.Sequential)]
public struct ServiceStatus
{
    public int dwServiceType;
    public ServiceState dwCurrentState;
    public int dwControlsAccepted;
    public int dwWin32ExitCode;
    public int dwServiceSpecificExitCode;
    public int dwCheckPoint;
    public int dwWaitHint;
};

namespace ImageService
{
    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ImageServer m_imageServer;          // The Image Server
        private IImageServiceModal modal;
        private IImageController controller;
       private ILoggingService logging;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        /// <summary>
        /// service constructor. creating event log and logging service.
        /// </summary>
        /// <param name="args"> for source and log name. </param>
        public ImageService (string[] args)
        {
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;

            logging = new LoggingService();
            logging.MessageRecieved += OnMessage;
        }

        /// <summary>
        /// starts service
        /// </summary>
        /// <param name="args"> for starting service. </param>
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            logging.Log("In OnStart", MessageTypeEnum.INFO);

            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            modal = new ImageServiceModal();
            controller = new ImageController(modal, logging);
            m_imageServer = new ImageServer(logging, modal, controller);
            logging.MessageRecieved += m_imageServer.UpdateLog;
            
        }
        /// <summary>
        /// writing an entry to log.
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for a message recieved event. </param>
        public void OnMessage(object sender, MessageRecievedEventArgs e)
        {
            EventLogEntryType type;
            if (e.Status == MessageTypeEnum.WARNING) {
                type = EventLogEntryType.Warning;
            } else if (e.Status == MessageTypeEnum.ERROR) {
                type = EventLogEntryType.FailureAudit;
            } else {
                type = EventLogEntryType.Information;
            }
            eventLog1.WriteEntry(e.Message, type);
        }

        /// <summary>
        /// The Event that will be activated upon timer finishing lapse.
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for an elapsed event. </param>
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        /// <summary>
        /// stops service
        /// </summary>
        protected override void OnStop()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            logging.Log("In OnStop", MessageTypeEnum.INFO);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            m_imageServer.CloseServer();
        }
    }
}
