// COPYRIGHT 1995-2025 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using System.Runtime.InteropServices;
using System;

namespace Esri.GameEngine.Authentication
{
    /// <summary>
    /// A callback invoked to present the OAuth logout page.
    /// </summary>
    internal delegate void ArcGISAuthenticationManagerOAuthUserLogoutEvent(ArcGISOAuthUserLogoutPrompt OAuthUserLogoutPrompt);
    
    internal delegate void ArcGISAuthenticationManagerOAuthUserLogoutEventInternal(IntPtr userData, IntPtr OAuthUserLogoutPrompt);
    
    internal class ArcGISAuthenticationManagerOAuthUserLogoutEventHandler : Unity.ArcGISEventHandler<ArcGISAuthenticationManagerOAuthUserLogoutEvent>
    {
        [Unity.MonoPInvokeCallback(typeof(ArcGISAuthenticationManagerOAuthUserLogoutEventInternal))]
        internal static void HandlerFunction(IntPtr userData, IntPtr OAuthUserLogoutPrompt)
        {
            if (userData == IntPtr.Zero)
            {
                return;
            }
            
            var callbackObject = (ArcGISAuthenticationManagerOAuthUserLogoutEventHandler)((GCHandle)userData).Target;
            
            if (callbackObject == null)
            {
                return;
            }
            
            var callback = callbackObject.m_delegate;
            
            if (callback == null)
            {
                return;
            }
            
            ArcGISOAuthUserLogoutPrompt localOAuthUserLogoutPrompt = null;
            
            if (OAuthUserLogoutPrompt != IntPtr.Zero)
            {
                localOAuthUserLogoutPrompt = new ArcGISOAuthUserLogoutPrompt(OAuthUserLogoutPrompt);
            }
            
            callback(localOAuthUserLogoutPrompt);
        }
    }
}