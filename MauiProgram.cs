using CCGGame.Services;
using CCGGame.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CCGGame;

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

		// Registrar servicios
        builder.Services.AddSingleton<ICardArtService, CardArtService>();
        builder.Services.AddSingleton<ICardDataService, CardDataService>();
        builder.Services.AddSingleton<IDeckService, DeckService>();
        builder.Services.AddTransient<IDuelService>(sp =>
        {
            var deckService = sp.GetRequiredService<IDeckService>();
            return new DuelService(deckService);
        });

		// Registrar ViewModels
		builder.Services.AddTransient<MainMenuViewModel>(sp =>
		{
			return new MainMenuViewModel(sp);
		});
		builder.Services.AddTransient<DeckBuilderViewModel>(sp =>
		{
            var cardDataService = sp.GetRequiredService<ICardDataService>();
            var deckService = sp.GetRequiredService<IDeckService>();
            var artService = sp.GetRequiredService<ICardArtService>();
			return new DeckBuilderViewModel(cardDataService, deckService, artService);
		});
		builder.Services.AddTransient<DuelViewModel>(sp =>
		{
            var duelService = sp.GetRequiredService<IDuelService>();
            var deckService = sp.GetRequiredService<IDeckService>();
            var cardDataService = sp.GetRequiredService<ICardDataService>();
            var artService = sp.GetRequiredService<ICardArtService>();
			return new DuelViewModel(duelService, deckService, cardDataService, artService);
		});

		// Registrar Views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<Views.DeckBuilderPage>();
		builder.Services.AddTransient<Views.DuelPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
