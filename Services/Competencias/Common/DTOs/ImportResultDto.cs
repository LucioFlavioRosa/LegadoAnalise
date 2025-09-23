namespace Peers.Moderno.Services.Competencias.Common.DTOs;

public class ImportResultDto
{
    public int LinhasInseridas { get; set; }
    public int LinhasAlteradas { get; set; }
    public int LinhasDesconsideradas { get; set; }
    public int LinhasComErro { get; set; }
    public List<string> Erros { get; set; } = new List<string>();
    public bool Sucesso => LinhasComErro == 0;
    
    public string MensagemResumo => 
        $"Competências importadas - Inseridas: {LinhasInseridas}, Alteradas: {LinhasAlteradas}, Desconsideradas: {LinhasDesconsideradas}, Com erro: {LinhasComErro}";
}