//using CFX.BatchJobs.DB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace CFX.Services.GenCardFile.Utility
{
    public class GenCardLogs : IDisposable
    {
        private class LogMessages
        {
            public string MainMethod { get; set; }
            public string CallerMethod { get; set; }
            public string Message { get; set; }
            public DateTime CreatedDate { get; set; }
            public byte Error { get; set; }
            public byte Warning { get; set; }
        }

        private string _runId;
        private string _batchJobId;
        private string _batchJobName;
        private string _mainMethod;
        private bool _success;
        private List<LogMessages> _logList;
        private List<Task> _openTasks;
        private ILogger _azureLogger;
        private bool _traceMode;

        private const string LOG_START_MESSAGE = "{0} - {1} - Start at :{2}";
        private const string LOG_END_MESSAGE = "{0} - {1} - Completed at :{2}";

        // Track whether Dispose has been called.
        private bool disposed = false;


        public bool TraceMode
        {
            get { return _traceMode; }
            set { _traceMode = value; }
        }

        public string RunId
        {
            get { return this._runId; }
        }


        public GenCardLogs(string runId,
                         string batchJobId,
                         string batchJobName,
                         string mainMethod,
                         ILogger azureLogger)
        {
            // INFO ABOUT ROUTINE
            this._runId = runId;
            this._batchJobId = batchJobId;
            this._batchJobName = batchJobName;
            this._logList = new List<LogMessages>();
            this._azureLogger = azureLogger;
            this._success = true;
            this._traceMode = bool.TryParse(Environment.GetEnvironmentVariable(Constants.TRACE_LOG), out bool isTraceMode) ? isTraceMode : true;

            this._openTasks = new List<Task>();

            // GET MAIN METHOD FOR LOG
            this._mainMethod = mainMethod;

            // START CREATE A LOG
            this.LogStartJob();
        }


        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~GenCardLogs()
        {
            Dispose(false);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Wait all open task
                foreach (var openTask in this._openTasks)
                {
                    if (!openTask.IsCompleted)
                    {
                        Task.WaitAll(openTask);
                    }
                }

                // Free any other managed objects here.
                this._traceMode = true;
                this.LogInformation(string.Format(LOG_END_MESSAGE, this._batchJobId, this._batchJobName, DateTime.UtcNow), "LogInDb_Dispose");
            }

            disposed = true;
        }

        // Public implementation of Dispose pattern callable by consumers.        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void InsertOrUpdateLogInDb(bool waitInsertUpdate)
        {
            string jsonLogs = JsonConvert.SerializeObject(this._logList);

            Task t1 = Task.Run(async () =>
            {
                await DbOperations.InsertOrUpdateLog(Constants.DATA_BASE_LOG_CONNECTION_STRING,
                                                     this._batchJobId,
                                                     this._batchJobName,
                                                     this._runId,
                                                     jsonLogs,
                                                     this._success ? 1 : 0,
                                                     this._azureLogger);
            });

            if (waitInsertUpdate)
            {
                Task.WaitAll(t1);
            }
            else
            {
                this._openTasks.Add(t1);
            }

        }

        private void LogStartJob()
        {
            LogMessages logMessage = new LogMessages();
            logMessage.MainMethod = this._mainMethod;
            logMessage.CallerMethod = string.Empty;
            logMessage.Message = string.Format(LOG_START_MESSAGE, this._batchJobId, this._batchJobName, DateTime.UtcNow);
            logMessage.Error = 0;
            logMessage.Warning = 0;
            logMessage.CreatedDate = DateTime.UtcNow;

            this._logList.Add(logMessage);

            this.InsertOrUpdateLogInDb(true);
        }



        public void LogInformation(string message, string callerMethod)
        {
            if (this._traceMode)
            {
                LogMessages logMessage = new LogMessages();
                logMessage.MainMethod = this._mainMethod;
                logMessage.CallerMethod = callerMethod;
                logMessage.Message = message;
                logMessage.Error = 0;
                logMessage.Warning = 0;
                logMessage.CreatedDate = DateTime.UtcNow;

                this._logList.Add(logMessage);

                this.InsertOrUpdateLogInDb(false);


                this._azureLogger.LogInformation(string.Format("{0} - BatchID: {1} - {2}", callerMethod, this._runId, message));
            }

        }

        public void LogWarning(string message, string callerMethod)
        {
            LogMessages logMessage = new LogMessages();
            logMessage.MainMethod = this._mainMethod;
            logMessage.CallerMethod = callerMethod;
            logMessage.Message = message;
            logMessage.Error = 0;
            logMessage.Warning = 1;
            logMessage.CreatedDate = DateTime.UtcNow;

            this._logList.Add(logMessage);

            this.InsertOrUpdateLogInDb(false);


            this._azureLogger.LogWarning(string.Format("{0} - BatchID: {1} - {2}", callerMethod, this._runId, message));


        }
        public void LogError(string message, string callerMethod)
        {
            this._success = false;

            LogMessages logMessage = new LogMessages();
            logMessage.MainMethod = this._mainMethod;
            logMessage.CallerMethod = callerMethod;
            logMessage.Message = message;
            logMessage.Error = 1;
            logMessage.Warning = 0;
            logMessage.CreatedDate = DateTime.UtcNow;

            this._logList.Add(logMessage);

            this.InsertOrUpdateLogInDb(false);


            this._azureLogger.LogError(string.Format("{0} - BatchID: {1} - {2}", callerMethod, this._runId, message));

        }

    }
}
