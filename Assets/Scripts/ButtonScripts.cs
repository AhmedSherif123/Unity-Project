using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.HPFramework;
using TMPro;
using Unity.Mathematics;

public class ButtonScripts : MonoBehaviour
{
    [Header("Target Object to Move")]
    public HPTransform targetObject;
    [Header("Button to be assigned")]
       public double heightStep = 30f;

    public void AddHeight()
    {
        if (targetObject != null)
        {
            double3 pos = (double3)targetObject.UniversePosition;
            pos.y += heightStep;
            targetObject.UniversePosition = pos;
        }
    }

    public void ReduceHeight()
    {
          double3 pos = (double3)targetObject.UniversePosition;
            pos.y -= heightStep;
            targetObject.UniversePosition = pos;
        
    }
}
