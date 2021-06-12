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
    private MouseInputController mouseInputController;
    private Collider _collider;
    private bool mouseHovering = false;
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
    }

    private void OnMouseEnter() 
    {
        var rend = GetComponent<Renderer>();
        rend.material.color = Color.red;
    }

    private void OnMouseExit()
    {
        var rend = GetComponent<Renderer>();
        rend.material.color = Color.green;
    }

    private void OnGUI()
    {
        string txt = playing ? "Stop!" : "Play1";
        if(GUI.Button(new Rect(30, 30, 150, 200), txt))
        {
            (playing ? playmodeEndEvent : playmodeStartEvent).Invoke();
            playing = !playing;
        }
    }

}
