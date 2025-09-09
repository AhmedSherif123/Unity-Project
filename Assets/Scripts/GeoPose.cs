// using UnityEngine;
// using TMPro;
// using UnityEngine.SpatialTracking;
// using UnityEngine.XR.ARFoundation;

// [RequireComponent(typeof(LineRenderer))]
// public class GeoPosePrinter : MonoBehaviour
// {
//     public Camera arCamera;
//     public float rayLength = 10f;
//     public float offsett;
//     [SerializeField] private LineRenderer lineRenderer;

//     [Header("UI")]
//     public TMP_Text headingText; // Assign in Inspector

//     [Header("Compass Filter")]
//     [Range(0.01f, 1f)]
//     public float filterStrength = 0.1f; // Lower = smoother but slower

//     private float smoothedHeading = 0;

//     void Start()
//     {
//         lineRenderer = GetComponent<LineRenderer>();

//         if (arCamera == null)
//             arCamera = Camera.main;

//         // Enable compass
//         Input.compass.enabled = true;

//         // Setup line renderer
//         lineRenderer.startWidth = 0.01f;
//         lineRenderer.endWidth = 1f;
//         lineRenderer.positionCount = 2;
//         lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//         lineRenderer.startColor = Color.green;
//         lineRenderer.endColor = Color.green;

//         // Initialize smoothed heading with first compass value
//         smoothedHeading = Input.compass.trueHeading;

//         // Get TPD reference
//     }

//     void Update()
//     {
//         if (arCamera == null) return;

//         // --- Get and smooth compass heading ---
//         float rawHeading = Input.compass.trueHeading;
//         smoothedHeading = Mathf.LerpAngle(smoothedHeading, rawHeading, filterStrength);

//         // --- Apply rotation to this object ---
//         transform.rotation = Quaternion.Euler(0, smoothedHeading, 0);

//         // --- Apply Y-axis only to AR camera ---
//         Vector3 arEuler = arCamera.transform.localEulerAngles;
//         arCamera.transform.localRotation = Quaternion.Euler(arCamera.transform.localRotation.x, smoothedHeading, arCamera.transform.localRotation.z);
//         Vector3 origin = new Vector3(transform.position.x, transform.position.y + offsett, transform.position.z);
//         Vector3 endPos = origin + transform.forward * rayLength;
//         lineRenderer.SetPosition(0, origin);
//         lineRenderer.SetPosition(1, endPos);

//         // --- Update UI ---
//         if (headingText != null)
//         {
//             headingText.text = "Heading: " + Mathf.RoundToInt(smoothedHeading) + "°";
//         }
//     }
// }


using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class GeoPosePrinter : MonoBehaviour
{
    public Camera arCamera;
    public float rayLength = 10f;
    public float offsett;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("UI")]
    public TMP_Text rotationText; // Assign in Inspector

    [Header("Filters")]
    [Range(0.01f, 1f)]
    public float gyroFilterStrength = 0.9f;    // keep fast response
    [Range(0.01f, 1f)]
    public float compassFilterStrength = 0.1f; // keep compass stable

    private Quaternion smoothedGyro = Quaternion.identity;
    private float smoothedHeading = 0f;

    // Orientation correction (Landscape Left)
    private static readonly Quaternion orientationCorrection = Quaternion.Euler(90, 0, 90);

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (arCamera == null)
            arCamera = Camera.main;

        // Enable sensors
        Input.gyro.enabled = true;
        Input.gyro.updateInterval = 0.01f; 
        Input.compass.enabled = true;
        
        // Setup line renderer
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // Init smoothing
        smoothedGyro = GyroToUnity(Input.gyro.attitude);
        smoothedHeading = Input.compass.trueHeading;
    }

    void Update()
    {
        if (arCamera == null) return;

        // --- Gyroscope orientation ---
        Quaternion rawGyro = GyroToUnity(Input.gyro.attitude);
        rawGyro = orientationCorrection * rawGyro;

        smoothedGyro = Quaternion.Slerp(smoothedGyro, rawGyro, gyroFilterStrength);

        // --- Compass heading (true north) ---
        float rawHeading = Input.compass.trueHeading;
        smoothedHeading = Mathf.LerpAngle(smoothedHeading, rawHeading, compassFilterStrength);

        // --- Apply rotations ---
        // Camera = full gyro orientation (fast & smooth)
        arCamera.transform.rotation = smoothedGyro;

        // Object = only Y from compass (true heading)
        transform.rotation = Quaternion.Euler(0, smoothedHeading, 0);

        // --- Draw forward line (based on object heading) ---
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + offsett, transform.position.z);
        Vector3 endPos = origin + transform.forward * rayLength;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);

        // --- Update UI ---
        if (rotationText != null)
        {
            Vector3 camEuler = smoothedGyro.eulerAngles;
            rotationText.text =
                $"Camera Pitch (X): {camEuler.x:F1}°\n" +
                $"Camera Yaw (Gyro): {camEuler.y:F1}°\n" +
                $"Camera Roll (Z): {camEuler.z:F1}°\n\n" +
                $"Object Y (Compass Heading): {smoothedHeading:F1}°";
        }
    }

    // Convert gyroscope right-handed coordinates to Unity's left-handed
    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}

