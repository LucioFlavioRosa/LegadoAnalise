using System.Collections.Generic;

namespace TriaSoftware.Util.Framework.Domain.Interface
{
    public interface IExportFile
    {
        byte[] GenerateExcel<T>(List<T> lstGeneric);
        byte[] GenerateCSV<T>(string delimiter, List<T> lstGeneric);
        //byte[] GeneratePDF(string html);
    }
}
