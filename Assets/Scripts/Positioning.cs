using TMPro;
using UnityEngine;
using Esri.HPFramework;
using Unity.Mathematics;
using System.Collections;
using Esri.ArcGISMapsSDK.Components;

public class Positioning : MonoBehaviour
{
    private const float EarthRadius = 6378137f;
    private const float UIUpdateInterval = 0.2f;
    public UnityLocationManager locationManager;

    [Header("UI")]
    [SerializeField] private TMP_Text loc;
    [SerializeField] private TMP_Text CamerasPos;

    [Header("Transforms")]
    [SerializeField] private HPTransform XRCamPos;
    [SerializeField] private HPTransform MapCamPos;
    [SerializeField] private HPTransform Mypos;
    [SerializeField] public HPTransform PluginPos;

    [Header("Debug / Fake Location")]
    public float fakeLat = 30.089610f;
    public float fakeLon = 31.70033f;

    public bool gpsReady = false;
    public bool usingFake = false;

    public Vector2 lastLatLon;
    public float lastUIUpdateTime;

    private void Start()
    {
        loc.text = "Loading...";

#if UNITY_EDITOR
        UseFakeLocation();
#else
        if (!Input.location.isEnabledByUser)
        {
            UseFakeLocation();
        }
        else
        {
            StartCoroutine(StartLocationService());
        }
#endif
    }

    private IEnumerator StartLocationService()
    {
        Input.location.Start(0.5f, 0.5f); // accuracy & distance
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            UseFakeLocation();
            yield break;
        }

        gpsReady = true;

        
    }

    private void Update()
    {
        if (!gpsReady && !usingFake)
        {
            if (Time.time - lastUIUpdateTime > UIUpdateInterval)
            {
                loc.text = "Waiting for GPS...";
                lastUIUpdateTime = Time.time;
            }
            return;
        }
        else
        {
            float currentLat = usingFake ? fakeLat : Input.location.lastData.latitude;
            float currentLon = usingFake ? fakeLon : Input.location.lastData.longitude;
            float currentAlt = usingFake ? (float)XRCamPos.UniversePosition.y : Input.location.lastData.altitude;

            Vector2 mercatorPos = LatLonToWebMercator(currentLat, currentLon);

          

          UpdateTransforms(mercatorPos);

            PluginPosition();



            // Update UI
            if (Time.time - lastUIUpdateTime > UIUpdateInterval)
            {
                UpdateUIText(currentLat, currentLon, currentAlt, mercatorPos);
                lastUIUpdateTime = Time.time;
            }
        }
        
    }
private void UpdateTransforms(Vector2 mercatorPos)
{
    // Update XR camera
    XRCamPos.UniversePosition = new double3(
        mercatorPos.x,
        XRCamPos.UniversePosition.y,
        mercatorPos.y
    );

    // Player
    Mypos.UniversePosition = new double3(
        XRCamPos.UniversePosition.x,
       Mypos.UniversePosition.y,
        XRCamPos.UniversePosition.z
    );

    // Map camera
    MapCamPos.UniversePosition = new double3(
        mercatorPos.x,
       MapCamPos.UniversePosition.y,
        mercatorPos.y
    );
      
    

    CamerasPos.text = $"Cam\nX = {MapCamPos.UniversePosition.x}\nY = {MapCamPos.UniversePosition.y:F0}\nZ= {MapCamPos.UniversePosition.z} ";
}

    private void UpdateUIText(float lat, float lon, float alt, Vector2 mercatorPos)
    {
        string source = usingFake ? "(FakeData)" : "(GPS)";
        loc.text = $"{source}\nLat: {lat:F6}\nLon: {lon:F6}\nAlt: {alt:F1} m" +
                   $"\nX: {XRCamPos.UniversePosition.x:F3} m\nY: {XRCamPos.UniversePosition.z:F3} m";

    }


    private void UseFakeLocation()
    {
        gpsReady = false;
        usingFake = true;
    }
    public void PluginPosition ()
    {
        double x;
        double y;
        if (!usingFake)
        {
            x = locationManager.CurrentLocation.Latitude;
            y = locationManager.CurrentLocation.Longitude;
        }
        else
        {
            x = fakeLat;
            y = fakeLon;
        }

        Vector2 LatLong =  LatLonToWebMercator((float)x, (float)y);
        PluginPos.UniversePosition = new double3(LatLong.x, PluginPos.UniversePosition.y, LatLong.y);
         }

    private Vector2 LatLonToWebMercator(float lat, float lon)
    {
        float x = EarthRadius * Mathf.Deg2Rad * lon;
        float y = EarthRadius * Mathf.Log(Mathf.Tan(Mathf.PI / 4f + Mathf.Deg2Rad * lat / 2f));
        return new Vector2(x, y);
    }
}
