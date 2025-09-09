namespace Business.Model
{
    public class PessoaModel : ModelBase
    {
        public string Nome { get; set; }
        public CargoModel Cargo { get; set; }
        public PessoaModel Mentor { get; set; }
    }
}
