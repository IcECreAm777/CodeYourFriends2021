using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTransform : MonoBehaviour
{
    [Header("Position")]
    [SerializeField]
    private float yPosition;
    [SerializeField]
    private float zPosition;

    private Vector3 position;

    [Header("Rotation")]
    [SerializeField]
    private float xRotation;

    private Vector3 rotation;

    private void rotateObject()
    {
        rotation = new Vector3(xRotation, 0, 0);
        transform.Rotate(rotation);
    }

    private void movePlatform()
    {
        position = new Vector3(0, yPosition, zPosition);
        transform.position += position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotateObject();
    }
}
