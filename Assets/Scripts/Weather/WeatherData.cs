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

public class WeatherData : MonoBehaviour
{

	private float timer=0;
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

	[SerializeField] private string current_weather;

	public void Awake()
	{
		location = "api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude + "&appid=" + API_key;
		latitude = locationService.latitude;
		longitude = locationService.longitude;
		Debug.Log(latitude + " - " + longitude);
		
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
				currentWeatherText.text = "Getting weather data";
				StartCoroutine(GetWeatherInfo());
				timer = minutesBetweenUpdate * 10;
			}
			else
			{
				timer -= Time.deltaTime;
			}
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
			currentWeatherText.text = "Network Error";
			//error
			yield break;
		}

		Info = JsonUtility.FromJson<WeatherInfo>(www.downloadHandler.text);
		currentWeatherText.text = "Current weather: " + Info.weather[0].main + ", " + Info.weather[0].description.ToString();// + ", " + Info.weather[0].icon;
		current_weather = Info.weather[0].description;

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
				break;
			case "snow":
				current_weather_int = (int)weather.snow;
				break;
			case "mist":
				current_weather_int = (int)weather.mist;
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
