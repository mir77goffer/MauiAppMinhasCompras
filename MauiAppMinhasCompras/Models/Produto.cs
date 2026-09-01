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
            set => _descricao = value?.Trim(); // não lança exceção
        }

        public double Quantidade { get; set; }
        public double Preco { get; set; }
        public double Total { get => Quantidade * Preco; }
    }
}
