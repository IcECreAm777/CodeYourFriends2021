using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoBehaviour : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField]
    private float radius = 5.0f;
    [SerializeField] 
    private float offsetX = -3.0f;
    
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
}
