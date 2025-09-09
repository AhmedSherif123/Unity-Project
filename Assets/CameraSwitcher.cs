using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera ARCamera;      // First camera (AR)
    public Camera ArcGISCamera;  // Second camera (Map)

    [Header("UI")]
    public Button switchButton;
    public TMP_Text btnText;

    private bool showingAR = true;

    void Start()
    {
        if (switchButton != null)
            switchButton.onClick.AddListener(SwitchCamera);

        // Start with AR camera active, ArcGIS camera active too for Option 2 (Depth)
        ARCamera.enabled = true;
        ArcGISCamera.enabled = true;

        // Depth order: AR camera renders first (lower depth)
        ARCamera.depth = 0;
        ArcGISCamera.depth = 1;

        // ClearFlags: AR camera draws normally, ArcGIS camera overlays
        
        ArcGISCamera.clearFlags = CameraClearFlags.Nothing;

        btnText.text = "Camera MAP"; // initial button text
    }

    public void SwitchCamera()
    {
        showingAR = !showingAR;

        if (showingAR)
        {
            ARCamera.enabled = true;
            ArcGISCamera.enabled = true;

            ARCamera.depth = 0;
            ArcGISCamera.depth = 1;

            ArcGISCamera.clearFlags = CameraClearFlags.Nothing;

            btnText.text = "Camera MAP";
        }
        else
        {
            ARCamera.enabled = true;
            ArcGISCamera.enabled = true;

            // Swap Depth so ArcGIS camera renders on top
            ARCamera.depth = 1;
            ArcGISCamera.depth = 0;

          
            ArcGISCamera.clearFlags = CameraClearFlags.Skybox;

            btnText.text = "Camera AR";
        }
    }
}
