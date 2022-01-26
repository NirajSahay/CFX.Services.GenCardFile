using System;
using System.Collections.Generic;
using System.Text;

namespace CFX.Services.GenCardFile.Utility
{
    public static class Constants
    {
       

        /// <summary>
        /// Script to Get all GenRent records not processed and not hit the maximum Tries
        /// </summary>
        public const string SELECT_RECORDS_FOR_FILE_TYPE = @"Select * From dbo.GenRent 
                                                             Where FileType = @FileType 
                                                             And IsProcessed = @IsProcessed
                                                             And NumberOfTries<@NumberOfTries";


       
        
        /// <summary>
        /// Script to Insert or Update Log for the actual BatchJob runing
        /// </summary>
        public const string INSERT_OR_UPDATE_LOG = @"IF EXISTS(Select Id from MessageLogs where RunId = @RunId)
                                                       Update MessageLogs Set
                                                        MessageLog = @MessageLog,
                                                        Success = @Success
                                                       Where RunId = @RunId
                                                    ELSE
                                                       Insert Into MessageLogs
                                                       (RunId,MessageLog,Success,BatchJobId,BatchJobName,CreatedDateTime)
                                                       values
                                                       (@RunId,@MessageLog,@Success,@BatchJobId,@BatchJobName,@CreatedDateTime);";

        
    }
}
}
