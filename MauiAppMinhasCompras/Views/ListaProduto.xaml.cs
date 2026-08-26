using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        // Lista "mestre": todos os produtos carregados do banco (fonte de verdade)
        List<Produto> todosProdutos = new List<Produto>();

        // Coleção exibida na tela — vinculada ao ListView.
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
            await CarregarProdutosAsync();
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
        }

        // Disparado a cada caractere digitado/apagado no SearchBar
        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltro(e.NewTextValue);
        }

        // Botão "Adicionar" na toolbar
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }

        // Botão "Somar" na toolbar
        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            double soma = lista.Sum(i => i.Total);
            string msg = $"O total é {soma:C}";
            DisplayAlertAsync("Total dos Produtos", msg, "OK");
        }

        // Ação de contexto "Remover" em cada item da lista
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = sender as MenuItem;
                Produto p = selecionado?.BindingContext as Produto;
                if (p == null)
                    return;

                bool confirm = await DisplayAlertAsync("Tem certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    todosProdutos.Remove(p);
                    lista.Remove(p);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        // Ao tocar em um item da lista, abre a tela de edição
        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Produto p = e.SelectedItem as Produto;
                if (p == null)
                    return;

                Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });

                // Desmarca o item selecionado (evita destaque preso ao voltar)
                lst_produtos.SelectedItem = null;
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        // Pull-to-refresh: recarrega tudo do banco e reaplica o filtro atual
        private async void lst_produtos_Refreshing(object sender, EventArgs e)
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
                lst_produtos.IsRefreshing = false;
            }
        }
    }
}
