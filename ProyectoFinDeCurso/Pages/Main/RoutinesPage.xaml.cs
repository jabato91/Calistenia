using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ProyectoFinDeCurso.Models;
using ProyectoFinDeCurso.Services;
using ProyectoFinDeCurso.ViewModels;

namespace ProyectoFinDeCurso.Pages.Main;

public partial class RoutinesPage : ContentPage
{
    private readonly DbService _dbService;
    public RoutinesPage(DbService dbService)
	{
        _dbService = dbService;
		InitializeComponent();

		BindingContext = new RoutinesFilterViewModel(_dbService);
    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        try
        {
            if ((sender as Grid)?.BindingContext is Exercise selectedExercise)
            {
                // Traemos la instancia actual desde la DB
                Exercise exerciseFromDb = await _dbService.GetExerciseById(selectedExercise.execiseID);

                var modalPage = new ContentPage
                {

                    BackgroundColor = Color.FromArgb("#D69C90"),
                    Content = new Frame
                    {
                        BackgroundColor = Color.FromArgb("#C44B4B"),
                        CornerRadius = 20,
                        Margin = 1,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Content = new VerticalStackLayout
                        {
                            Padding = 4,
                            Children =
            {
                new Button
                {
                    Text = "X",
                    FontFamily = "calculator",
                    BackgroundColor =  Color.FromArgb("#BF9F9F"),
                     CornerRadius = 999,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    FontSize =17,
                    Padding = new Thickness(0),
                     Margin = new Thickness(0, 5, 5, 0),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    Command = new Command(async () =>
                        await Navigation.PopModalAsync())
                },
                new Label
                {
                    Text = exerciseFromDb.name,
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                },
                new Label
                {
                    Text = exerciseFromDb.description,
                    FontSize = 13,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black,
                    Margin = 1.5
                },
                new Frame
                            {
                                CornerRadius = 15,
                                HasShadow = true,
                                BackgroundColor = Colors.Black,
                                Padding = 0,
                                Margin = new Thickness(2,3,2,4),
                                Content = new MediaElement
                                {
                                    Source = MediaSource.FromResource("prueba.mp4"),
                                    Aspect = Aspect.AspectFit,
                                    ShouldShowPlaybackControls = true,
                                    HeightRequest = 325,
                                    WidthRequest = 500
                                }
                            },

                }
                        }
                    }
                };

                // Muestra la ventana modal
                await Navigation.PushModalAsync(modalPage);
            }
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error amigable
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al abrir el ejercicio:\n{ex.Message}",
                "OK"
            );
        }
    }
    private async void OnExpanded(object sender, ExpandedChangedEventArgs e)
    {
        if (sender is not Expander expander)
            return;

        if (expander.Content is not VisualElement content)
            return;

        if (e.IsExpanded)
        {
            // 🔹 ANIMACIÓN AL ABRIR
            content.Opacity = 0;
            content.TranslationY = -20;
            await Task.WhenAll(
                content.FadeTo(1, 250, Easing.SinInOut),
                content.TranslateTo(0, 0, 250, Easing.SinInOut)
            );
        }
       
    }

}