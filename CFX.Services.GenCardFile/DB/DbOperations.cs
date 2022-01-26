using CFX.DAL.Interfaces;
using CFX.DAL.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CFX.Services.GenCardFile.Utility;
using CFX.Services.GenCardFile.Models;


namespace CFX.Services.GenCardFile.DB
{
    public static class DbOperations
    {
        /// <summary>
        /// Insert or Update the Log Record (RunID)
        /// </summary>
        /// <param name="databaseConnection">String Connection to Data Base</param>
        /// <param name="batchJobId">Batch Job Id</param>
        /// <param name="batchJobName">Batch Job Name</param>
        /// <param name="runId">Run Id</param>
        /// <param name="jsonLogs">Json with all Message Logs</param>
        /// <param name="success">Its proccess with Success</param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public static async Task<bool> InsertOrUpdateLog(string dataBaseConnection,
                                                         string batchJobId,
                                                         string batchJobName,
                                                         string runId,
                                                         string jsonLogs,
                                                         int success,
                                                         ILogger azureLogger)
        {
            try
            {
                int recordsAffected = 0;

                var recordLog = new
                {
                    RunId = runId,
                    MessageLog = jsonLogs,
                    Success = success,
                    BatchJobId = batchJobId,
                    BatchJobName = batchJobName,
                    CreatedDateTime = DateTime.UtcNow
                };

                using (SqlConnection sqlConnection = new SqlConnection(dataBaseConnection))
                {
                    recordsAffected = await sqlConnection.ExecuteAsync(Constants.INSERT_OR_UPDATE_LOG, recordLog).ConfigureAwait(false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        

    }
}
