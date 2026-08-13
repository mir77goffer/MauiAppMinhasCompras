using Microsoft.Maui.Devices;

namespace MauiAppMinhasCompras.Views
{
    public partial class ResponsivoPage : ContentPage
    {
        public ResponsivoPage()
        {
            InitializeComponent();
            AjustarLayout();
        }

        private void AjustarLayout()
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            double largura = displayInfo.Width / displayInfo.Density;

            // Ajusta espaçamento conforme largura da tela
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

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Sucesso", "Produto salvo com layout responsivo!", "OK");
        }
    }
}
