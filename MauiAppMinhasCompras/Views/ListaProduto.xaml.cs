using Microsoft.Maui.Devices; // Necessário para acessar informações do dispositivo, como largura da tela

using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

        public ListaProduto()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista;

            AjustarLayout(); // Chama o método para ajustar o layout com base na largura da tela
        }

        // Método para ajustar o layout com base na largura da tela
        private void AjustarLayout()
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            double largura = displayInfo.Width / displayInfo.Density;

            if (largura < 400)
            {
                layoutPrincipal.Spacing = 8;
                layoutPrincipal.Padding = new Thickness(10);
            }
            else if (largura < 600)
            {
                layoutPrincipal.Spacing = 12;
                layoutPrincipal.Padding = new Thickness(15);
            }
            else
            {
                layoutPrincipal.Spacing = 20;
                layoutPrincipal.Padding = new Thickness(30);
            }
        }

        // Demais métodos (OnAppearing, busca, soma, remover, etc.) permanecem iguais



        protected async override void OnAppearing()
        {
            try
            {
                lista.Clear();
                List<Produto> tmp = await App.Db.GetAll();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }

        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string q = e.NewTextValue;
                lst_produtos.IsRefreshing = true;
                lista.Clear();
                List<Produto> tmp = await App.Db.Search(q);
                tmp.ForEach(i => lista.Add(i));
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

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            double soma = lista.Sum(i => i.Total);
            string msg = $"O total é {soma:C}";
            DisplayAlertAsync("Total dos Produtos", msg, "OK");
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecinado = sender as MenuItem;
                Produto p = selecinado.BindingContext as Produto;

                bool confirm = await DisplayAlertAsync("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    lista.Remove(p);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Produto p = e.SelectedItem as Produto;
                Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private async void lst_produtos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear();
                List<Produto> tmp = await App.Db.GetAll();
                tmp.ForEach(i => lista.Add(i));
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
