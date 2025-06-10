namespace MauiAppWeather;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Affiche la page de prévision
    /// </summary>
    /// <param name="sender"><see cref="object"/> L'objet source de l'événement</param>
    /// <param name="e"><see cref="EventArgs"/> Les arguments de l'événement</param>
    private void GetStartedBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new WeatherPage());
    }
}