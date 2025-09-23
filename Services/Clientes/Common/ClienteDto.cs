using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Services.Clientes.Common;

public class ClienteDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do cliente é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome do cliente deve ter no máximo 200 caracteres")]
    public string Cliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gestor do cliente é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do gestor deve ter no máximo 100 caracteres")]
    public string GestorCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sócio responsável é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um sócio responsável")]
    public int IdAssociadoResponsavel { get; set; }

    public bool Ativo { get; set; } = true;
}