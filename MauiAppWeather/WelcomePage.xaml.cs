namespace MauiAppWeather;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Affiche la page de pr�vision
    /// </summary>
    /// <param name="sender"><see cref="object"/> L'objet source de l'�v�nement</param>
    /// <param name="e"><see cref="EventArgs"/> Les arguments de l'�v�nement</param>
    private void GetStartedBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new WeatherPage());
    }
}