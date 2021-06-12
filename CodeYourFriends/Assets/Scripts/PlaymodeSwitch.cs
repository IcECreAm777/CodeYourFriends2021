using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlaymodeSwitch : MonoBehaviour
{

    [Header("Event settings")]
    [SerializeField]
    private UnityEvent playmodeStartEvent;
    [SerializeField]
    private UnityEvent playmodeEndEvent;

    private bool playing = false;

    //components
    private Collider _collider;

    //input
    private MouseInputController mouseInputController;
    private bool mouseHovering = false;
    private bool mouseBtnPressed = false;
    private void Start()
    {
        var player = GameObject.Find("Capsule");
        mouseInputController = player.GetComponent<MouseInputController>();

        _collider = GetComponent<Collider>();
        Debug.Log(_collider);
    }

    void Update()
    {
        bool hovered = false;
        foreach(var result in mouseInputController.collidersUnderMouse)
        {
            if(result.collider == _collider)
            {
                hovered = true;
                break;
            }
        }

        if (hovered != mouseHovering)
        {
            mouseHovering = hovered;
            if(hovered) OnMouseEnter();
            else OnMouseExit();
        }

        if(mouseInputController.mouseButtonDown != mouseBtnPressed)
        {
            mouseBtnPressed = mouseInputController.mouseButtonDown;
            if(mouseBtnPressed) OnMouseClicked();
        }
    }

    private void OnMouseEnter() 
    {
        //TODO: effect for mouse hovering started
        var rend = GetComponent<Renderer>();
        rend.material.color = Color.red;
    }

    private void OnMouseExit()
    {
        //TODO: effect for mouse hovering stopped
        var rend = GetComponent<Renderer>();
        rend.material.color = playing ? Color.green : Color.gray;
    }
    
    private void OnMouseClicked()
    {
        if(!mouseHovering) return;
        (playing ? playmodeEndEvent : playmodeStartEvent).Invoke();
        playing = !playing;
    }

}
