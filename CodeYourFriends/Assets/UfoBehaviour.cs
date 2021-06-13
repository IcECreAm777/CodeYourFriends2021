using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UfoBehaviour : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField]
    private float radius = 5.0f;
    [SerializeField] 
    private float offsetX = -3.0f;
    
    [Header("UI")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Text broadcastText;

    private GameObject _parent;
    private GameObject _goal;

    private void Awake()
    {
        _parent = transform.parent.gameObject;
    }

    public void StartPointingToGoal()
    {
        _goal = GameObject.Find("GoalPost");
        StartCoroutine(PointToGoal());
    }

    public void StopPointingToGoal()
    {
        StopCoroutine(PointToGoal());
    }

    public void BroadcastPopupMessage(string message, float displayDuration, 
        bool travels = false, Vector3 travelPos = new Vector3(), float travelTime = -1.0f)
    {
        broadcastText.text = message;
        if (travels)
        {
            StartCoroutine(TravelToPoint(travelPos, travelTime, displayDuration));
            return;
        }

        StartCoroutine(DisplayDialogue(displayDuration));
    }
    
    private IEnumerator PointToGoal()
    {
        for (;;)
        {
            var parentPosition = _parent.transform.position;
            var goalPosition = _goal.transform.position;
            var direction = goalPosition - parentPosition;
            var playerDist = Vector3.Distance(parentPosition, goalPosition);
            var ufoDist = Vector3.Distance(parentPosition, goalPosition) > radius ? radius : playerDist;
            var ufoPos = direction.normalized * ufoDist;
            ufoPos.x = offsetX;
            transform.position = parentPosition + ufoPos;
            yield return null;
        }
    }

    private IEnumerator TravelToPoint(Vector3 pos, float travelTime, float displayDuration)
    {
        var time = 0.0f;
        var currentUfoPos = transform.position;

        while (time < travelTime)
        {
            transform.position = Vector3.Lerp(currentUfoPos, pos, 1 / travelTime * time);
            time += Time.deltaTime;
            yield return null;
        }
        
        yield return StartCoroutine(DisplayDialogue(displayDuration));

        time = 0.0f;
        while (time < travelTime)
        {
            transform.position = Vector3.Lerp(currentUfoPos, pos, 1 / travelTime * time);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DisplayDialogue(float displayTime)
    {
        canvas.enabled = true;
        var waitForSeconds = new WaitForSeconds(displayTime);
        yield return waitForSeconds;
        canvas.enabled = false;
    }
}
