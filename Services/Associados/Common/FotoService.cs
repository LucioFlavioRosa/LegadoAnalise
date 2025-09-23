using Microsoft.AspNetCore.Components.Forms;

namespace Peers.Moderno.Services.Associados.Common;

public interface IFotoService
{
    Task<string> ProcessarFotoAsync(IBrowserFile file);
    bool ValidarExtensao(string fileName);
    string ObterMimeType(string extension);
}

public class FotoService : IFotoService
{
    private readonly string[] _extensoesPermitidas = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
    private readonly long _tamanhoMaximo = 5 * 1024 * 1024; // 5MB

    public async Task<string> ProcessarFotoAsync(IBrowserFile file)
    {
        if (file == null)
            throw new ArgumentException("Nenhum arquivo fornecido");

        if (file.Size > _tamanhoMaximo)
            throw new ArgumentException("Arquivo muito grande. Tamanho máximo: 5MB");

        var extension = Path.GetExtension(file.Name).ToLower();
        if (!ValidarExtensao(file.Name))
            throw new ArgumentException("Extensão de arquivo não permitida. Use: jpg, jpeg, png, gif, bmp");

        var mimeType = ObterMimeType(extension);
        
        using var stream = file.OpenReadStream(_tamanhoMaximo);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        
        var bytes = memoryStream.ToArray();
        var base64 = Convert.ToBase64String(bytes);
        
        return $"data:{mimeType};base64,{base64}";
    }

    public bool ValidarExtensao(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return _extensoesPermitidas.Contains(extension);
    }

    public string ObterMimeType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "image/jpeg"
        };
    }
}