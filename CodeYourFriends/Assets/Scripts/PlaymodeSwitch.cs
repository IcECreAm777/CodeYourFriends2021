using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaymodeSwitch : MonoBehaviour
{

    [Header("Event settings")]
    [SerializeField]
    private UnityEvent playmodeStartEvent;
    [SerializeField]
    private UnityEvent playmodeEndEvent;

    private bool playing = false;
    void Start() { }

    void Update() { }

    private void OnGUI()
    {
        Debug.Log("GUI");
        string txt = playing ? "Stop!" : "Play1";
        if(GUI.Button(new Rect(30, 30, 150, 200), txt))
        {
            (playing ? playmodeEndEvent : playmodeStartEvent).Invoke();
            playing = !playing;
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("mouseEnter");
        var rend = GetComponent<Renderer>();
        rend.material.color = Color.red;
    }

    void OnMouseOver()
    {
        Debug.Log("mouseOver");
    }

    void OnMouseExit()
    {
        Debug.Log("mouseExit");
        var rend = GetComponent<Renderer>();
        rend.material.color = Color.gray;
    }
}
