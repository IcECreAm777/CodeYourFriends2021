using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BroadcastUfoEvent : UnityEvent<PopupScriptableObject>
{
}

public class UfoMessageInvoker : MonoBehaviour
{
    [Header("Events")] 
    [SerializeField] 
    private bool onSpawn = false;
    [SerializeField]
    private float timeAfterSpawn = -1.0f;
    [SerializeField]
    private BroadcastUfoEvent customEvent;

    [Header("Message Properties")] 
    [SerializeField]
    private PopupScriptableObject popup;

    private UfoBehaviour _ufo;

    protected void Awake()
    {
        _ufo = GameObject.Find("UFO").GetComponent<UfoBehaviour>();
    }

    protected void Start()
    {
        if (onSpawn)
        {
            _ufo.BroadcastPopupMessage(popup.message, popup.displayTime, popup.travelsToPoint, 
                popup.travelTarget, popup.travelTime);
            return;
        }

        if (timeAfterSpawn < 0.0f) return;

        StartCoroutine(WaitToBroadcast());
    }

    private IEnumerator WaitToBroadcast()
    {
        var wait = new WaitForSeconds(timeAfterSpawn);
        yield return wait;
        _ufo.BroadcastPopupMessage(popup.message, popup.displayTime, popup.travelsToPoint, 
            popup.travelTarget, popup.travelTime);
    }

    public void OnEvent(PopupScriptableObject ufoMessage)
    {
        _ufo.BroadcastPopupMessage(ufoMessage.message, ufoMessage.displayTime, ufoMessage.travelsToPoint, 
            ufoMessage.travelTarget, ufoMessage.travelTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEvent(popup);
        Destroy(gameObject);
    }
}
