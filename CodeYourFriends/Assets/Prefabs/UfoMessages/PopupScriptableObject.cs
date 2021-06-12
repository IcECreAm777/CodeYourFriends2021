using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UFO Dialogue", menuName = "UFO Dialogue")]
public class PopupScriptableObject : ScriptableObject
{
    public string message;
    public float displayTime;
    public bool travelsToPoint;
    public Vector3 travelTarget;
    public float travelTime;
}
