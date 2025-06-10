using MauiAppWeather.Models;
using MauiAppWeather.Services;

namespace MauiAppWeather;

public partial class WeatherPage : ContentPage
{
    public List<List> weatherList;

    private double _latitude, _longitude;

    /// <summary>
    /// Initialise la page m�t�o ses composants
    /// </summary>
    public WeatherPage()
    {
        InitializeComponent();
        weatherList = new();
    }

    /// <summary>
    /// R�cup�re la localisation de l'utilisateur et les donn�es m�t�o correspondantes
    /// Cette m�thode est appel�e automatiquement lorsque la page devient visible
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetLocation();
        await GetWeatherByDataLocation(_latitude, _longitude);
    }

    /// <summary>
    /// R�cup�re la position actuelle de l'utilisateur latitude et longitude
    /// </summary>
    /// <returns><see cref="Task"/> Une t�che asynchrone repr�sentant l'op�ration de localisation</returns>
    public async Task GetLocation()
    {
        try
        {
            Location? location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            if (location == null)
            {
                await DisplayAlert("Localisation indisponible", "Impossible d'obtenir la position actuelle. Veuillez v�rifier vos param�tres de localisation.", "Ok");
                return;
            }

            _latitude = location.Latitude;
            _longitude = location.Longitude;
        }
        catch (FeatureNotEnabledException)
        {
            await DisplayAlert("Localisation d�sactiv�e", "La localisation est d�sactiv�e sur votre appareil. Activez-la pour obtenir la m�t�o locale.", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permission manquante", "L'application n'a pas la permission d'acc�der � votre position.", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "Ok");
        }
    }

    /// <summary>
    /// �v�nement d�clench� lors du touch� sur l'encadr� de localisation
    /// Actualise la localisation et les donn�es m�t�o en cons�quence
    /// </summary>
    /// <param name="sender"><see cref="object"/> L'objet source de l'�v�nement</param>
    /// <param name="e"><see cref="EventArgs"/> Les arguments de l'�v�nement</param>
    private async void TapLocationTapped(object sender, EventArgs e)
    {
        await GetLocation();
        await GetWeatherByDataLocation(_latitude, _longitude);
    }

    /// <summary>
    /// R�cup�re les donn�es m�t�o depuis une API en fonction de la latitude et de la longitude sp�cifi�es
    /// </summary>
    /// <param name="latitude"><see cref="double"/> La latitude utilis�e pour la requ�te</param>
    /// <param name="longitude"><see cref="double"/> La longitude utilis�e pour la requ�te</param>
    /// <returns><see cref="Task"/> Une t�che asynchrone repr�sentant l'op�ration de r�cup�ration</returns>
    public async Task GetWeatherByDataLocation(double latitude, double longitude)
    {
        Root? result = await ApiServices.GetWeather(latitude, longitude);

        if (string.IsNullOrWhiteSpace(result?.ToString())) return;

        UpdateUI(result);
    }

    /// <summary>
    /// �v�nement d�clench� lors du touch� sur le bouton de recherche
    /// Affiche une bo�te de dialogue pour entrer une ville et charge les donn�es m�t�o correspondantes
    /// </summary>
    /// <param name="sender"><see cref="object"/> L'objet source de l'�v�nement</param>
    /// <param name="e"><see cref="EventArgs"/> Les arguments de l'�v�nement</param>
    private async void SearchBtnClicked(object sender, EventArgs e)
    {
        string response;

        while (true)
        {
            response = await DisplayPromptAsync(
             title: $"Ville actuelle {LblCity.Text}",
             message: "",
             placeholder: "Entrer une ville",
             accept: "Rechercher",
             cancel: "Annuler"
         );

            if (response == null) return;

            if (string.IsNullOrWhiteSpace(response))
            {
                await DisplayAlert("Ville manquante", "Entrez le nom d'une ville", "Ok");
                continue;
            }
            Root? weatherData = await ApiServices.GetWeatherByCity(response);

            if (weatherData == null)
            {
                await DisplayAlert("Ville introuvable", $"La ville \"{response}\" est inconnue ou mal orthographi�e", "Ok");
                continue;
            }
            break;
        }
        await GetWeatherDataByCity(response);
    }

    /// <summary>
    ///  R�cup�re les donn�es m�t�o pour une ville donn�e via l'API
    /// </summary>
    /// <param name="city"><see cref="string"/> Le nom de la ville pour laquelle obtenir la m�t�o</param>
    /// <returns><see cref="Task"/> Une t�che asynchrone repr�sentant l'op�ration de r�cup�ration</returns>
    public async Task GetWeatherDataByCity(string city)
    {
        Root? result = await ApiServices.GetWeatherByCity(city);

        if (string.IsNullOrWhiteSpace(result.ToString())) return;

        UpdateUI(result);
    }

    /// <summary>
    /// Met � jour l'interface utilisateur avec les donn�es m�t�o re�ues
    /// </summary>
    /// <param name="result"><see cref="Root"/> Les donn�es m�t�o � afficher</param>
    private void UpdateUI(dynamic result)
    {
        foreach (List item in result.List)
        {
            weatherList.Add(item);
        }
        LblCity.Text = result.City.Name;
        LblDescription.Text = result.List[0].Weather[0].Description;
        LblTemperature.Text = $"{result.List[0].Main.Temperature} �C";
        LblHumidity.Text = $"{result.List[0].Main.Humidity} %";
        LblWind.Text = $"{result.List[0].Wind.Speed} km/h";
        ImgWeatherIcon.Source = result.List[0].Weather[0].CustomIcon;
        CvWeather.ItemsSource = weatherList;
    }
}