using MauiAppMinhasCompras.Models;
using SQLite;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);

            _conn.CreateTableAsync<Produto>().Wait();

            // Compatibilidade com bancos criados antes do campo Categoria
            try
            {
                _conn.ExecuteAsync(
                    "ALTER TABLE Produto ADD COLUMN Categoria TEXT DEFAULT 'Sem categoria'")
                    .Wait();
            }
            catch
            {
                // A coluna já existe; nenhuma ação é necessária.
            }
        }

        public Task<int> Insert(Produto p)
        {
            if (string.IsNullOrWhiteSpace(p.Categoria))
                p.Categoria = "Sem categoria";

            return _conn.InsertAsync(p);
        }

        public Task<int> Update(Produto p)
        {
            if (string.IsNullOrWhiteSpace(p.Categoria))
                p.Categoria = "Sem categoria";

            string sql = @"
                UPDATE Produto
                SET Descricao = ?,
                    Categoria = ?,
                    Quantidade = ?,
                    Preco = ?
                WHERE Id = ?";

            return _conn.ExecuteAsync(
                sql,
                p.Descricao,
                p.Categoria,
                p.Quantidade,
                p.Preco,
                p.Id);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>()
                .DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>()
                .ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = @"
                SELECT *
                FROM Produto
                WHERE Descricao LIKE ?
                   OR Categoria LIKE ?";

            string termo = $"%{q}%";

            return _conn.QueryAsync<Produto>(
                sql,
                termo,
                termo);
        }
    }
}