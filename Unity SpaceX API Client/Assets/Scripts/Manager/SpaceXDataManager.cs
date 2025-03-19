using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace SpaceXLaunchesBrowser
{
    public class SpaceXDataManager : MonoBehaviour
    {
        public List<Launch> Launches;
        public static Sprite CurrentShipSprite;

        private readonly string _shipUrl = "https://api.spacexdata.com/v4/ships/";
        private readonly string _rocketUrl = "https://api.spacexdata.com/v4/rockets/";
        private readonly string _launchesUrl = "https://api.spacexdata.com/v4/launches";

        private bool _launchesAdded;

        public void GetAllLaunches()
        {
            if (_launchesAdded) return;

            StartCoroutine(GetLaunches());
            _launchesAdded = true;
        }
        public void GetAllShips(Launch launch)
        {
            if (launch.ShipsAdded) return;

            StartCoroutine(GetShips(launch));

            launch.ShipsAdded = true;
        }

        public void LoadImage(string url)
        {
            StartCoroutine(DownloadImage(url));
        }
        private IEnumerator GetLaunches()
        {
            SpaceXLaunchesBrowserCanvasManager.OnStartLoading.Invoke();

            string url = _launchesUrl;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    string data = www.downloadHandler.text;

                    List<Launch> launches = JsonHelper.FromJson<Launch>(data);

                    if (launches != null)
                    {
                        Launches = launches.ToList();

                        foreach (var launch in launches)
                        {
                            if (!string.IsNullOrEmpty(launch.RocketId))
                            {
                                launch.Rocket = new Rocket();
                                yield return StartCoroutine(GetRocket(launch.RocketId, launch.Rocket));
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Error parsing JSON: " + data);
                    }
                }
            }

            SpaceXLaunchesBrowserCanvasManager.OnEndLoading.Invoke();
            SpaceXLaunchesBrowserCanvasManager.OnGetLaunches.Invoke();
        }
        private IEnumerator GetRocket(string rocketId, Rocket rocket)
        {
            string url = _rocketUrl + rocketId;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    string rocketData = www.downloadHandler.text;

                    try
                    {
                        JsonConvert.PopulateObject(rocketData, rocket);
                    }
                    catch (JsonException ex)
                    {
                        Debug.LogError("Error deserializing rocket data: " + ex.Message + "\nJSON: " + rocketData);
                    }
                }
            }
        }
        private IEnumerator GetShips(Launch launch)
        {
            SpaceXLaunchesBrowserCanvasManager.OnStartLoading.Invoke();

            List<Ship> ships = new();
            foreach (string shipId in launch.ShipIds)
            {
                string url = _shipUrl + shipId;
                using (UnityWebRequest www = UnityWebRequest.Get(url))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Error fetching ship data: " + www.error);
                    }
                    else
                    {
                        string shipData = www.downloadHandler.text;

                        try
                        {
                            Ship ship = new();
                            JsonConvert.PopulateObject(shipData, ship);

                            if (ship != null)
                            {
                                ships.Add(ship);
                            }
                            else
                            {
                                Debug.LogError("Ship deserialization failed. Check JSON and Ship class. JSON: " + shipData);
                            }

                        }
                        catch (JsonException ex)
                        {
                            Debug.LogError("JSON Exception (Ship): " + ex.Message + "\nJSON: " + shipData);
                        }
                    }
                }
            }
            launch.Ships = ships.ToList();

            SpaceXLaunchesBrowserCanvasManager.OnEndLoading.Invoke();
            SpaceXLaunchesBrowserCanvasManager.OnGetShips.Invoke(launch);
        }

        private IEnumerator DownloadImage(string url)
        {
            SpaceXLaunchesBrowserCanvasManager.OnStartLoading.Invoke();

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error loading image: " + www.error);
                }
                else
                {
                    Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    CurrentShipSprite = sprite;
                }
            }

            SpaceXLaunchesBrowserCanvasManager.OnEndLoading.Invoke();
            SpaceXLaunchesBrowserCanvasManager.OnGetImage.Invoke();
        }

        #region JSON classes
        [Serializable]
        public class Launch
        {
            [JsonProperty("name")] private readonly string name;
            [JsonProperty("upcoming")] private readonly bool upcoming;
            [JsonProperty("rocket")] private readonly string rocketId;
            [JsonProperty("payloads")] private readonly List<string> payloads;
            [JsonProperty("ships")] private readonly List<string> shipIds;

            public string Name { get => name; }
            public bool Upcoming { get => upcoming; }
            public string RocketId { get => rocketId; }
            public List<string> Payloads { get => payloads; }
            public List<string> ShipIds { get => shipIds; }

            [HideInInspector] public List<Ship> Ships;
            [HideInInspector] public Rocket Rocket;
            [HideInInspector] public bool ShipsAdded;
        }

        [Serializable]
        public class Rocket
        {
            [JsonProperty("name")] private readonly string name;
            [JsonProperty("country")] private readonly string country;
            public string Name { get => name; }
            public string Country { get => country; }
        }

        [Serializable]
        public class Ship
        {
            [JsonProperty("name")] private readonly string name;
            [JsonProperty("type")] private readonly string type;
            [JsonProperty("home_port")] private readonly string home_port;
            [JsonProperty("image")] private readonly string image;
            [JsonProperty("launches")] private readonly List<string> launches;

            public string Name { get => name; }
            public string Type { get => type; }
            public string Home_port { get => home_port; }
            public string Image { get => image; }
            public List<string> Launches { get => launches; }
        }
        #endregion

        #region Helper class for JsonUtility
        public static class JsonHelper
        {
            public static List<T> FromJson<T>(string json)
            {
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
        }
        #endregion
    }
}
