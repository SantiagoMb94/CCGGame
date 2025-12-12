using CCGGame.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CCGGame;

public partial class MainPage : ContentPage
{
	public MainPage(MainMenuViewModel? viewModel = null)
	{
		InitializeComponent();
		
		// Obtener el ViewModel del contenedor de servicios si no se proporciona
		if (viewModel == null)
		{
			var serviceProvider = Application.Current?.Handler?.MauiContext?.Services;
			if (serviceProvider != null)
			{
				viewModel = serviceProvider.GetService<MainMenuViewModel>();
			}
		}
		
		if (viewModel != null)
		{
			BindingContext = viewModel;
		}
	}
}

