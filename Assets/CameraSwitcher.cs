using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Esri.ArcGISMapsSDK.Components;

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

        // Start with AR mode
        SetMapMode();
    }

    public void SwitchCamera()
    {
        showingAR = !showingAR;

        if (showingAR)
            SetARMode();
        else
            SetMapMode();
    }

    private void SetARMode()
    {
        // AR on top
        ARCamera.enabled = true;
        ArcGISCamera.enabled = true;

        ARCamera.depth = 0;
        ArcGISCamera.depth = 1;
        ArcGISCamera.clearFlags = CameraClearFlags.Nothing;

    

        btnText.text = "Switch to MAP";
    }

    private void SetMapMode()
    {
        // Map on top
        ARCamera.enabled = true;
        ArcGISCamera.enabled = true;

        ARCamera.depth = 1;
        ArcGISCamera.depth = 0;
        ArcGISCamera.clearFlags = CameraClearFlags.Skybox;


        btnText.text = "Switch to AR";
    }
}
