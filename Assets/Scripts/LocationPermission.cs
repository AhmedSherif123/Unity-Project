using UnityEngine;
using UnityEngine.Android;
using TMPro;

public class LocationPermissionRequester : MonoBehaviour
{
    public TMP_Text permissions;

    void Start()
    {
        RequestPermissions();
        ShowGrantedPermissions();
    }

    void RequestPermissions()
    {
#if UNITY_ANDROID
        // Fine Location
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            Permission.RequestUserPermission(Permission.FineLocation);

        // Coarse Location
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            Permission.RequestUserPermission(Permission.CoarseLocation);

        // Camera
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);

        // Background Location (Android 10+)
        string backgroundLocation = "android.permission.ACCESS_BACKGROUND_LOCATION";
        if (!Permission.HasUserAuthorizedPermission(backgroundLocation))
            Permission.RequestUserPermission(backgroundLocation);

        // Foreground Service
        string foregroundService = "android.permission.FOREGROUND_SERVICE";
        if (!Permission.HasUserAuthorizedPermission(foregroundService))
            Permission.RequestUserPermission(foregroundService);
#endif
    }

    void ShowGrantedPermissions()
    {
#if UNITY_ANDROID
        string granted = "Granted Permissions:\n";

        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            granted += "- Fine Location\n";

        if (Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            granted += "- Coarse Location\n";

        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            granted += "- Camera\n";

        if (Permission.HasUserAuthorizedPermission("android.permission.ACCESS_BACKGROUND_LOCATION"))
            granted += "- Background Location\n";

        if (Permission.HasUserAuthorizedPermission("android.permission.FOREGROUND_SERVICE"))
            granted += "- Foreground Service\n";

        permissions.text = granted;
#else
        permissions.text = "Not running on Android.";
#endif
    }
}
