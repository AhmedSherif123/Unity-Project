using TMPro;
using UnityEngine;
using Esri.ArcGISMapsSDK.Components;

public class EnableDisable : MonoBehaviour
{
    public Positioning positioning;
    public TMP_Text Map_State;
    public bool state;

    void Start()
    {
        if (positioning != null)
        {
            state = false;
            Map_State.text = "Disabled";
        }
    }

    // Toggle the state of the positioning component
    public void ChangeState()
    {
        if (positioning != null)
        {
            // Toggle state
            state = !state;
            positioning.enabled = state;
            if (state == false)
            {
                Map_State.text = "Disabled";
            }
            else
            {
                Map_State.text = "Enabled";
           }
        }
    }
}
