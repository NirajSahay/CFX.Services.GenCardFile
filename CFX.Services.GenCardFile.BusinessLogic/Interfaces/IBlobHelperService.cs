using System.Threading.Tasks;

namespace CFX.Services.GenCardFile.BusinessLogic.Interfaces
{
    public interface IBlobHelperService
    {
        public interface IBlobHelperService
        {
            /// <summary>
            /// This method returns the statement as string from cloud
            /// </summary>
            /// <param name="statementFileName"></param>
            /// <returns>statement as string or a blank string if there's an error</returns>
            Task<string> GetStatementAsString(string statementFileName);
        }
    }
}
