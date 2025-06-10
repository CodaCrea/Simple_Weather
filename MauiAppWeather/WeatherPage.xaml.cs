using MauiAppWeather.Models;
using MauiAppWeather.Services;

namespace MauiAppWeather;

public partial class WeatherPage : ContentPage
{
    public List<List> weatherList;

    private double _latitude, _longitude;

    /// <summary>
    /// Initialise la page météo ses composants
    /// </summary>
    public WeatherPage()
    {
        InitializeComponent();
        weatherList = new();
    }

    /// <summary>
    /// Récupère la localisation de l'utilisateur et les données météo correspondantes
    /// Cette méthode est appelée automatiquement lorsque la page devient visible
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetLocation();
        await GetWeatherByDataLocation(_latitude, _longitude);
    }

    /// <summary>
    /// Récupère la position actuelle de l'utilisateur latitude et longitude
    /// </summary>
    /// <returns><see cref="Task"/> Une tâche asynchrone représentant l'opération de localisation</returns>
    public async Task GetLocation()
    {
        try
        {
            Location? location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            if (location == null)
            {
                await DisplayAlert("Localisation indisponible", "Impossible d'obtenir la position actuelle. Veuillez vérifier vos paramètres de localisation.", "Ok");
                return;
            }

            _latitude = location.Latitude;
            _longitude = location.Longitude;
        }
        catch (FeatureNotEnabledException)
        {
            await DisplayAlert("Localisation désactivée", "La localisation est désactivée sur votre appareil. Activez-la pour obtenir la météo locale.", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permission manquante", "L'application n'a pas la permission d'accéder à votre position.", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "Ok");
        }
    }

    /// <summary>
    /// Événement déclenché lors du touché sur l'encadré de localisation
    /// Actualise la localisation et les données météo en conséquence
    /// </summary>
    /// <param name="sender"><see cref="object"/> L'objet source de l'événement</param>
    /// <param name="e"><see cref="EventArgs"/> Les arguments de l'événement</param>
    private async void TapLocationTapped(object sender, EventArgs e)
    {
        await GetLocation();
        await GetWeatherByDataLocation(_latitude, _longitude);
    }

    /// <summary>
    /// Récupère les données météo depuis une API en fonction de la latitude et de la longitude spécifiées
    /// </summary>
    /// <param name="latitude"><see cref="double"/> La latitude utilisée pour la requête</param>
    /// <param name="longitude"><see cref="double"/> La longitude utilisée pour la requête</param>
    /// <returns><see cref="Task"/> Une tâche asynchrone représentant l'opération de récupération</returns>
    public async Task GetWeatherByDataLocation(double latitude, double longitude)
    {
        Root? result = await ApiServices.GetWeather(latitude, longitude);

        if (string.IsNullOrWhiteSpace(result?.ToString())) return;

        UpdateUI(result);
    }

    /// <summary>
    /// Événement déclenché lors du touché sur le bouton de recherche
    /// Affiche une boîte de dialogue pour entrer une ville et charge les données météo correspondantes
    /// </summary>
    /// <param name="sender"><see cref="object"/> L'objet source de l'événement</param>
    /// <param name="e"><see cref="EventArgs"/> Les arguments de l'événement</param>
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
                await DisplayAlert("Ville introuvable", $"La ville \"{response}\" est inconnue ou mal orthographiée", "Ok");
                continue;
            }
            break;
        }
        await GetWeatherDataByCity(response);
    }

    /// <summary>
    ///  Récupère les données météo pour une ville donnée via l'API
    /// </summary>
    /// <param name="city"><see cref="string"/> Le nom de la ville pour laquelle obtenir la météo</param>
    /// <returns><see cref="Task"/> Une tâche asynchrone représentant l'opération de récupération</returns>
    public async Task GetWeatherDataByCity(string city)
    {
        Root? result = await ApiServices.GetWeatherByCity(city);

        if (string.IsNullOrWhiteSpace(result.ToString())) return;

        UpdateUI(result);
    }

    /// <summary>
    /// Met à jour l'interface utilisateur avec les données météo reçues
    /// </summary>
    /// <param name="result"><see cref="Root"/> Les données météo à afficher</param>
    private void UpdateUI(dynamic result)
    {
        foreach (List item in result.List)
        {
            weatherList.Add(item);
        }
        LblCity.Text = result.City.Name;
        LblDescription.Text = result.List[0].Weather[0].Description;
        LblTemperature.Text = $"{result.List[0].Main.Temperature} °C";
        LblHumidity.Text = $"{result.List[0].Main.Humidity} %";
        LblWind.Text = $"{result.List[0].Wind.Speed} km/h";
        ImgWeatherIcon.Source = result.List[0].Weather[0].CustomIcon;
        CvWeather.ItemsSource = weatherList;
    }
}