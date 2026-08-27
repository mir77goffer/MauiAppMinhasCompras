using System.Globalization;

namespace MauiAppMinhasCompras.Helpers
{
    public static class NumberHelper
    {
        // Converte o texto digitado em número, aceitando tanto vírgula quanto ponto
        // como separador decimal — independente da cultura/idioma configurado no aparelho.
        public static bool TryParseDecimal(string texto, out double valor)
        {
            valor = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.Trim();

            // 1) Tenta com a cultura atual do aparelho (ex.: pt-BR usa vírgula)
            if (double.TryParse(texto, NumberStyles.Any, CultureInfo.CurrentCulture, out valor))
                return true;

            // 2) Tenta trocando vírgula por ponto e usando cultura invariante (en-US)
            string comPonto = texto.Replace(",", ".");
            if (double.TryParse(comPonto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                return true;

            // 3) Tenta trocando ponto por vírgula e usando cultura pt-BR
            string comVirgula = texto.Replace(".", ",");
            return double.TryParse(comVirgula, NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"), out valor);
        }
    }
}
