namespace Models
{
    public class UsuarioLogado
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public int IdEmpresa { get; set; }
        public string Nome { get; set; }
        public bool IsLogged { get; set; }
        public int IdCargo { get; set; }
        public int IdPerfil { get; set; }
    }
}