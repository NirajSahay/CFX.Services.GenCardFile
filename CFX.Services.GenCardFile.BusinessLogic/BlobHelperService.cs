using CFX.Secrets;
//using CFX.Services.Document.Helper.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using CFX.Services.GenCardFile.BusinessLogic.Interfaces;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CFX.Services.GenCardFile.BusinessLogic
{
    class BlobHelperService : IBlobHelperService
    {
        /// <summary>
        /// Defines the keyvaultProvider
        /// </summary>
        private readonly IKeyvaultProvider keyvaultProvider;

        /// <summary>
        /// Gets the configuration
        /// </summary>
        private readonly IConfiguration configuration;

        public BlobHelperService(IKeyvaultProvider keyvaultProvider, IConfiguration configuration)
        {
            this.keyvaultProvider = keyvaultProvider;
            this.configuration = configuration;
        }

        /// <summary>
        /// This method returns the statement as string from cloud
        /// </summary>
        /// <param name="statementFileName"></param>
        /// <returns>statement as string or a blank string if there's an error</returns>
        public async Task<string> GetStatementAsString(string statementFileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(keyvaultProvider.GetSecretKeyAsync(Constants.StatementBlobConnectionString));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(configuration["BlobStorage:ContainerName"]);
            string test = "this is test file";
            StringBuilder fileText = new StringBuilder();
            // STARTING FILE CREATION
            using (StringWriter stringWriter = new StringWriter(fileText))
            {
                await stringWriter.FlushAsync();
                fileText.Clear();
                List<string> transactionRecords = new List<string> { "test_file1", "test_file2" };
                foreach (var record in transactionRecords)
                {
                    stringWriter.WriteLine(record);
                }
                var numberOfLines = Regex.Matches(fileText.ToString(), Environment.NewLine).Count;

                if (numberOfLines > 0)
                {                    
                        cLog.LogInformation(string.Format("File {0} has been created with {1} lines", fileName, numberOfLines), nameof(CreateMearsFile));
                        await Common.UploadFileToBlob(fileText, fileName, genRentRecord.AccountNumber, this._keyvaultProvider, cLog, Constants.MRS);


                        // IF FTP URL IS SET TRY TO SEND FILE
                        if (_ftpUrlMears != null && _ftpUrlMears.Trim() != string.Empty)
                        {
                            // PUT FILE TO A FTP
                            cLog.LogInformation(string.Format("Trying put file: {0} on FTP:{1} - UserName:{2} - PWD:{3}", fileName, this._ftpUrlMears, this._ftpUser_Mears, this._ftpPwd_Mears), nameof(CreateMearsFile));

                            try
                            {
                                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpUrlMears + fileName);
                                request.Method = WebRequestMethods.Ftp.UploadFile;

                                request.Credentials = new NetworkCredential(_ftpUser_Mears, _ftpPwd_Mears);
                                request.UsePassive = true;
                                request.UseBinary = true;
                                request.KeepAlive = true;

                                StreamReader sourceStream = new StreamReader(fileText.ToString());
                                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                                sourceStream.Close();
                                request.ContentLength = fileContents.Length;

                                Stream requestStream = request.GetRequestStream();
                                requestStream.Write(fileContents, 0, fileContents.Length);
                                requestStream.Close();

                                FtpWebResponse response = (FtpWebResponse)request.GetResponse();


                                await DbOperations.UpdateRecordsAsync(Constants.MRS,
                                                                      this._databaseConnectionString,
                                                                      genRentRecord,
                                                                      fileName,
                                                                      true,
                                                                      true,
                                                                      foData.Count,
                                                                      string.Empty,
                                                                      cLog);



                                //BlobClient blobClient = containerClient.UploadBlobAsync("testBlob", )
                                //    //GetBlobClient(statementFileName);
                                //byte[] bytes;
                                //using (MemoryStream ms = new MemoryStream())
                                //{
                                //    await blobClient.DownloadToAsync(ms);
                                //    bytes = ms.ToArray();
                                //}
                                //if (bytes != null && bytes.Length > 0)
                                //{
                                //    return Convert.ToBase64String(bytes);
                                //}
                                //return string.Empty;
                            }
    }
}
