using MauiAppWeather.Models;
using Newtonsoft.Json;

namespace MauiAppWeather.Services
{
    public class ApiServices
    {
        /// <summary>
        /// Récupère les prévisions météorologiques selon la latitude et la longitude spécifiées.
        /// </summary>
        /// <param name="latitude"><see cref="double"/> Latitude du lieu</param>
        /// <param name="longitude"><see cref="double"/> Longitude du lieu</param>
        /// <returns><see cref="Root"/> Une tâche asynchrone retournant un objet représentant les données météo ou null en cas d'erreur</returns>
        public static async Task<Root?> GetWeather(double latitude, double longitude)
        {
            AppConfig? _config = await AppConfig.LoadKeyAsync();
            string url = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&appid={_config.ApiKey}&units=metric&lang=fr";
            return await GetWeatherApi(url);
        }

        /// <summary>
        /// Récupère les prévisions météorologiques selon le nom de la ville spécifié.
        /// </summary>
        /// <param name="city"><see cref="string"/> Nom de la ville</param>
        /// <returns><see cref="Root"/> Une tâche asynchrone retournant un objet représentant les données météo ou null en cas d'erreur</returns>
        public static async Task<Root?> GetWeatherByCity(string city)
        {
            AppConfig? _config = await AppConfig.LoadKeyAsync();
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_config.ApiKey}&units=metric&lang=fr";
            return await GetWeatherApi(url);
        }

        /// <summary>
        /// Envoie une requête HTTP GET à l'URL spécifiée pour récupérer les données météo.
        /// </summary>
        /// <param name="url"><see cref="string"/> URL complète de l'API OpenWeatherMap</param>
        /// <returns><see cref="Root"/> Une tâche asynchrone retournant un objet désérialisé depuis la réponse JSON ou null en cas de réponse vide ou invalide</returns>
        private static async Task<Root?> GetWeatherApi(string url)
        {
            HttpClient httpClient = new();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                string content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content)) return null;

                return JsonConvert.DeserializeObject<Root>(content);
            }
            catch
            {
                return null;
            }
        }
    }
}
