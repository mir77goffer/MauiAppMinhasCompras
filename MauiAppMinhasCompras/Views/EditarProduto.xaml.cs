using Microsoft.Maui.Devices;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views
{
    public partial class EditarProduto : ContentPage
    {
        public EditarProduto()
        {
            InitializeComponent();
            AjustarLayout();
        }

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

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                Produto produto_anexado = BindingContext as Produto;
                Produto p = new Produto
                {
                    Id = produto_anexado.Id,
                    Descricao = txt_descricao.Text,
                    Quantidade = Convert.ToDouble(txt_quantidade.Text),
                    Preco = Convert.ToDouble(txt_preco.Text)
                };

                await App.Db.Update(p);
                await DisplayAlertAsync("Sucesso!", "Registro Atualizado", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
    }
}
