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
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	public static class ArcGISSpatialFeatureFilterInstanceDataExtensions
	{
		public static void ApplyToSerializedProperty(this ArcGISSpatialFeatureFilterInstanceData spatialFeatureFilterInstanceData, SerializedProperty serializedProperty)
		{
			spatialFeatureFilterInstanceData.IsEnabled.ApplyToSerializedProperty(serializedProperty.FindPropertyRelative("IsEnabled"));
			spatialFeatureFilterInstanceData.Polygons.ApplyToSerializedProperty(serializedProperty.FindPropertyRelative("Polygons"));
			spatialFeatureFilterInstanceData.Type.ApplyToSerializedProperty(serializedProperty.FindPropertyRelative("Type"));
		}
	}
}
