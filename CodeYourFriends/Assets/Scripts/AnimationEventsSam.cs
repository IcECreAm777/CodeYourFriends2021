using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation events for Sam.
/// The Animator component will search for animation events here.
/// </summary>
public class AnimationEventsSam : MonoBehaviour
{
    [Header("SoundClips")]
    [SerializeField] 
    private AudioClip footstepSound;
    
    // components
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void PlayFootstepSound()
    {
        _audio.clip = footstepSound;
        _audio.Play();
    }
}
