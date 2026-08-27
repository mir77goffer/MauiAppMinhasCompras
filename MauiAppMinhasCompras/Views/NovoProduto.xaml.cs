using MauiAppMinhasCompras.Helpers;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views
{
    public partial class NovoProduto : ContentPage
    {
        public NovoProduto()
        {
            InitializeComponent();
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_descricao.Text))
                {
                    await DisplayAlertAsync("Ops", "Informe a descrição do produto.", "OK");
                    return;
                }

                if (!NumberHelper.TryParseDecimal(txt_quantidade.Text, out double quantidade))
                {
                    await DisplayAlertAsync("Ops", "Informe uma quantidade válida (ex: 2 ou 2,5).", "OK");
                    return;
                }

                if (!NumberHelper.TryParseDecimal(txt_preco.Text, out double preco))
                {
                    await DisplayAlertAsync("Ops", "Informe um preço válido (ex: 15,90 ou 15.90).", "OK");
                    return;
                }

                Produto p = new Produto
                {
                    Descricao = txt_descricao.Text,
                    Quantidade = quantidade,
                    Preco = preco
                };

                await App.Db.Insert(p);
                await DisplayAlertAsync("Sucesso!", "Produto adicionado.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
    }
}
