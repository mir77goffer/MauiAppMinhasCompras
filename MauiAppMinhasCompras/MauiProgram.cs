using Microsoft.Extensions.Logging;

namespace MauiAppMinhasCompras
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ADICIONE este trecho dentro de MauiProgram.CreateMauiApp(), ANTES de "return builder.Build();"
            // Ele corrige o teclado Numeric no Android, que por padrão não habilita
            // a tecla de separador decimal ('.' ou ',').

#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("EntryNumericDecimalFix", (handler, view) =>
            {
                if (view.Keyboard == Keyboard.Numeric)
                {
                    handler.PlatformView.InputType =
                        Android.Text.InputTypes.ClassNumber
                        | Android.Text.InputTypes.NumberFlagDecimal
                        | Android.Text.InputTypes.NumberFlagSigned;
                }
            });
#endif

            return builder.Build();
        }
    }
}
