using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        // Lista "mestre": todos os produtos carregados do banco (fonte de verdade)
        List<Produto> todosProdutos = new List<Produto>();

        // Coleção exibida na tela — vinculada ao CollectionView.
        // Qualquer Add/Remove/Clear aqui atualiza a interface automaticamente.
        ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

        public ListaProduto()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            await CarregarProdutosAsync();

            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
        }

        // Carrega (ou recarrega) os produtos do banco e reaplica o filtro atual
        async Task CarregarProdutosAsync()
        {
            try
            {
                todosProdutos = await App.Db.GetAll();
                AplicarFiltro(txt_search.Text);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        // Filtra em memória sobre a lista mestre e sincroniza a ObservableCollection
        // exibida na tela, sem precisar consultar o banco a cada tecla digitada.
        void AplicarFiltro(string termo)
        {
            termo = (termo ?? string.Empty).Trim();

            IEnumerable<Produto> resultado = string.IsNullOrEmpty(termo)
                ? todosProdutos
                : todosProdutos.Where(p =>
                    !string.IsNullOrEmpty(p.Descricao) &&
                    p.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase));

            lista.Clear();
            foreach (var produto in resultado)
                lista.Add(produto);

            AtualizarResumo();
        }

        // Atualiza os "chips" com a quantidade de itens exibidos e o valor total
        void AtualizarResumo()
        {
            int quantidade = lista.Count;
            double total = lista.Sum(i => i.Total);

            lbl_contagem.Text = quantidade == 1 ? "1 produto" : $"{quantidade} produtos";
            lbl_total.Text = $"Total: {total:C}";
        }

        // Disparado a cada caractere digitado/apagado no SearchBar
        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltro(e.NewTextValue);
        }

        // Botão flutuante "+"
        private void OnAdicionarClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }

        // Swipe para a esquerda no card -> "Remover"
        private async void SwipeItem_Remover_Invoked(object sender, EventArgs e)
        {
            try
            {
                SwipeItem item = sender as SwipeItem;
                Produto p = item?.BindingContext as Produto;
                if (p == null)
                    return;

                bool confirm = await DisplayAlertAsync("Tem certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    todosProdutos.Remove(p);
                    lista.Remove(p);
                    AtualizarResumo();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        // Toque em um card -> abre a tela de edição
        private void lst_produtos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Produto p = e.CurrentSelection.FirstOrDefault() as Produto;
                if (p == null)
                    return;

                Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });

                // Desmarca o item selecionado (evita o card ficar "destacado" ao voltar)
                lst_produtos.SelectedItem = null;
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        // Pull-to-refresh: recarrega tudo do banco e reaplica o filtro atual
        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            try
            {
                await CarregarProdutosAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
            finally
            {
                refreshView.IsRefreshing = false;
            }
        }
    }
}
