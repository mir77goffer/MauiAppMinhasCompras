using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MauiAppMinhasCompras.Views
{
    public partial class RelatorioCategorias : ContentPage
    {
        readonly ObservableCollection<RelatorioCategoria> relatorio =
            new();

        public RelatorioCategorias()
        {
            InitializeComponent();

            lst_relatorio.ItemsSource = relatorio;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await CarregarRelatorioAsync();
        }

        private async Task CarregarRelatorioAsync()
        {
            try
            {
                List<Produto> produtos = await App.Db.GetAll();

                List<RelatorioCategoria> agrupado = produtos
                    .GroupBy(p =>
                        string.IsNullOrWhiteSpace(p.Categoria)
                            ? "Sem categoria"
                            : p.Categoria.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g => new RelatorioCategoria
                    {
                        Categoria = g.Key,
                        Total = g.Sum(p => p.Total),
                        QuantidadeProdutos = g.Count()
                    })
                    .OrderByDescending(r => r.Total)
                    .ToList();

                relatorio.Clear();

                foreach (RelatorioCategoria item in agrupado)
                    relatorio.Add(item);

                double totalGeral = agrupado.Sum(r => r.Total);

                lbl_total_geral.Text = totalGeral.ToString(
                    "C",
                    CultureInfo.CurrentCulture);

                lbl_resumo.Text = agrupado.Count == 1
                    ? "1 categoria"
                    : $"{agrupado.Count} categorias";
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(
                    "Ops",
                    ex.Message,
                    "OK");
            }
        }
    }
}