using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

enum weather
{
    clear_sky,
    few_clouds,
    scattered_clouds,
    broken_clouds,
    shower_rain,
    rain,
    thunderstorm,
    snow,
    mist
}

enum weather_particle_effect
{
    rain,
    mist,
    heavy_rain,
    thunder_strom,
    snow

}

public class WeatherData : MonoBehaviour
{

    private float timer = 0;
    public float minutesBetweenUpdate;
    public WeatherInfo Info;
    //https://home.openweathermap.org/ 3418d745d339c19425544cbbe6756565
    public string API_key;

    private bool locationInitialized;
    public Text currentWeatherText;

    private float latitude;
    private float longitude;
    public LocationService locationService;

    private string location;
    public int current_weather_int;

    public int overwrite_weather = -1;


    [NamedArrayAttribute(new string[] { "rain", "mist", "heavy_rain", "thunder_strom", "snow"})]
    public GameObject[] particle_effects;

    [SerializeField] private string current_weather;

    public void Awake()
    {
        location = "api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude + "&appid=" + API_key;
        latitude = locationService.latitude;
        longitude = locationService.longitude;
        //Debug.Log(latitude + " - " + longitude);

        if (latitude == 0 && longitude == 0)
        {
            location = "api.openweathermap.org/data/2.5/weather?q=Saukkola,uusimaa&appid=" + API_key;
        }
        locationInitialized = true;
    }
    void Update()
    {

        if (locationInitialized)
        {
            if (timer <= 0)
            {
                if (currentWeatherText != null)
                {
                    currentWeatherText.text = "Getting weather data";
                }
                StartCoroutine(GetWeatherInfo());
                timer = minutesBetweenUpdate * 60;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        if (overwrite_weather != -1)
        {
            GetWeatherInfo();
        }
    }
    private IEnumerator GetWeatherInfo()
    {
        var www = new UnityWebRequest(location)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            if (currentWeatherText != null)
            {
                currentWeatherText.text = "Network Error";
            }
            //error
            yield break;
        }

        Info = JsonUtility.FromJson<WeatherInfo>(www.downloadHandler.text);
        if (currentWeatherText != null)
        {
            currentWeatherText.text = "Current weather: " + Info.weather[0].main + ", " + Info.weather[0].description.ToString();// + ", " + Info.weather[0].icon;
        }
        current_weather = Info.weather[0].description;

        for (int i = 0; i < particle_effects.Length; i++)
        {
            particle_effects[i].GetComponent<ParticleSystem>().Stop();
        }

        Debug.Log("Weather: " + current_weather);

        #region - Convert weather strings to ints
        switch (current_weather)
        {
            case "clear sky":
                current_weather_int = (int)weather.clear_sky;
                break;
            case "few clouds":
                current_weather_int = (int)weather.few_clouds;
                break;
            case "scattered clouds":
                current_weather_int = (int)weather.scattered_clouds;
                break;
            case "broken clouds":
                current_weather_int = (int)weather.broken_clouds;
                break;
            case "shower rain":
                current_weather_int = (int)weather.shower_rain;
                break;
            case "rain":
                current_weather_int = (int)weather.rain;
                break;
            case "thunderstorm":
                current_weather_int = (int)weather.thunderstorm;
                particle_effects[(int)weather_particle_effect.rain].GetComponent<ParticleSystem>().Play();
                break;
            case "snow":
                current_weather_int = (int)weather.snow;
                break;
            case "mist":
                current_weather_int = (int)weather.mist;
                particle_effects[(int)weather_particle_effect.mist].GetComponent<ParticleSystem>().Play();
                break;
        }
        #endregion

        current_weather_int = (overwrite_weather == -1) ? current_weather_int : overwrite_weather;

        switch (current_weather_int)
        {
            case (int)weather.clear_sky:
                break;
            case (int)weather.few_clouds:
                break;
            case (int)weather.scattered_clouds:
                break;
            case (int)weather.broken_clouds:
                break;
            case (int)weather.shower_rain:
                particle_effects[(int)weather_particle_effect.rain].GetComponent<ParticleSystem>().Play();
                ParticleSystem particleSystem = particle_effects[(int)weather_particle_effect.rain].GetComponent<ParticleSystem>();
                //var emission = particleSystem.emission;
                //emission.rateOverTime = 330;
                //particle_effects[(int)weather_particle_effect.mist].GetComponent<ParticleSystem>().Play();
                break;
            case (int)weather.rain:
                particle_effects[(int)weather_particle_effect.rain].GetComponent<ParticleSystem>().Play();
                //ParticleSystem particleSystem2 = particle_effects[(int)weather_particle_effect.rain].GetComponent<ParticleSystem>();
                //var emission2 = particleSystem2.emission;
                //emission.rateOverTime = 130;
                break;
            case (int)weather.thunderstorm:
                particle_effects[(int)weather_particle_effect.thunder_strom].GetComponent<ParticleSystem>().Play();
                particle_effects[(int)weather_particle_effect.rain].GetComponent<ParticleSystem>().Play();
                break;
            case (int)weather.snow:
                particle_effects[(int)weather_particle_effect.snow].GetComponent<ParticleSystem>().Play();
                break;
            case (int)weather.mist:
                particle_effects[(int)weather_particle_effect.mist].GetComponent<ParticleSystem>().Play();
                break;
        }
        
}
}
/*
clear sky
few clouds
scattered clouds
broken clouds
shower rain
rain
thunderstorm
snow	
mist
 */
[Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[Serializable]
public class WeatherInfo
{
    public int id;
    public string name;
    public List<Weather> weather;
}
