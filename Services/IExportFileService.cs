using System.Collections.Generic;

namespace Peers.Moderno.Services
{
    public interface IExportFileService
    {
        byte[] GenerateExcelConsideracoesMentor<T>(string fileName, List<T> data);
    }
}