using MauiAppMinhasCompras.Helpers;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views
{
    public partial class EditarProduto : ContentPage
    {
        public EditarProduto()
        {
            InitializeComponent();
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            try
            {
                Produto? produto_anexado = BindingContext as Produto;
                if (produto_anexado == null)
                    return;

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

                Produto p = new ()
                {
                    Id = produto_anexado.Id,
                    Descricao = txt_descricao.Text, // agora pode ser editado sem crash
                    Quantidade = quantidade,
                    Preco = preco
                };

                await App.Db.Update(p);
                await DisplayAlertAsync("Sucesso!", "Registro atualizado.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
    }
}
