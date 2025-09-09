namespace Business.Util
{
    public class UsuarioLogado
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string FotoNome { get; set; }
        public int IdEmpresa { get; set; }
        public bool IsLogged { get; set; }
        public int IdCargo { get; set; }
        public int IdPerfil { get; set; }
    }
}
