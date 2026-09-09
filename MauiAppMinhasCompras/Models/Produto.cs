using SQLite;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        string _descricao;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Descricao
        {
            get => _descricao;
            set
            {
                if (value != null)
                    _descricao = value.Trim(); // não lança exceção
                else
                    _descricao = null;
            }
        }

        public string Categoria { get; set; } = "Sem categoria";

        public double Quantidade { get; set; }

        public double Preco { get; set; }

        [Ignore]
        public double Total => Quantidade * Preco;
    }
}
