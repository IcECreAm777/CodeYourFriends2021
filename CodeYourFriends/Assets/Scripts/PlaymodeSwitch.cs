using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlaymodeSwitch : MonoBehaviour
{
    [Header("Player Spawn")] 
    [SerializeField]
    public GameObject spawnPoint;
    
    [Header("Event settings")]
    [SerializeField]
    public UnityEvent playmodeStartEvent;
    [SerializeField]
    public UnityEvent playmodeEndEvent;

    //components
    private Collider _collider;

    private bool _playMode = true;

    private void Start()
    {
        PlayModeStart();
    }

    public void ToggleMode()
    {
        if (_playMode)
        {
            EditModeStart();
            return;
        }
        
        PlayModeStart();
    }

    public void PlayModeStart()
    {
        playmodeStartEvent.Invoke();
        GameObject.Find("nyancat").transform.localScale = new Vector3(1, 1, 1);
        _playMode = true;
    }

    public void EditModeStart()
    {
        playmodeEndEvent.Invoke();
        GameObject.Find("nyancat").transform.localScale = new Vector3(10,10,10);
        _playMode = false;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPoint.transform.position;
    }
}
