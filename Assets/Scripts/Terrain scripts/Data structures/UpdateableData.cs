using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    void OnValidate() 
    {
        
    }

    public void NotifyOfUpdatedValues()
    {
        if(OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
}
