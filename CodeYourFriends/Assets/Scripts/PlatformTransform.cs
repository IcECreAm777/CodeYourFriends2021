using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTransform : MonoBehaviour
{
    [Header("Speed")] 
    [SerializeField] 
    private float timeToTravel = 2.0f;
    [SerializeField] 
    private float timeToRest = 0.0f;

    [Header("MovementBounds")]
    [SerializeField]
    private float yOffset;
    [SerializeField]
    private float zOffset;
    
    // non editor properties
    private Vector3 _startPos;
    private Vector3 _endPos;

    protected void Awake()
    {
        InitializePositions();
        StartMoving();
    }

    protected void OnCollisionEnter(Collision other)
    {
        other.transform.parent = transform;
    }

    protected void OnCollisionExit(Collision other)
    {
        other.transform.SetParent(null);
    }

    private void InitializePositions()
    {
        _startPos = transform.localPosition;
        _endPos = _startPos + new Vector3(0, yOffset, zOffset);
    }

    public void StopMoving()
    {
        StopCoroutine(Move());
    }

    public void StartMoving()
    {
        InitializePositions();
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        var wait = new WaitForSeconds(timeToRest);
        
        while (true)
        {
            var timeTraveled = 0.0f;
            while (timeTraveled < timeToTravel)
            {
                var newPos = Vector3.Lerp(_startPos, _endPos, 1 / timeToTravel * timeTraveled);
                transform.localPosition = newPos;
                timeTraveled += Time.deltaTime;
                yield return null;
            }

            yield return wait;

            timeTraveled = 0.0f;
            while (timeTraveled < timeToTravel)
            {
                var newPos = Vector3.Lerp(_endPos, _startPos, 1 / timeToTravel * timeTraveled);
                transform.localPosition = newPos;
                timeTraveled += Time.deltaTime;
                yield return null;
            }

            yield return wait;
        }
    }
}
