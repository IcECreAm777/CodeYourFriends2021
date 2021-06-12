using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTransform : MonoBehaviour
{
    [Header("Position")]
    [SerializeField]
    private float yDelta;
    [SerializeField]
    private float zDelta;

    [Header("MovementBounds")]
    [SerializeField]
    private float yBoundLower;
    [SerializeField]
    private float yBoundUpper;
    [SerializeField]
    private float zBoundLower;
    [SerializeField]
    private float zBoundUpper;

    private Vector3 positionDelta;

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
        positionDelta = new Vector3(0, yDelta, zDelta);

        if (yBoundLower < transform.position.y && transform.position.y < yBoundUpper) transform.position += positionDelta;
        if (transform.position.y >= yBoundUpper || transform.position.y <= yBoundLower)
        {
            yDelta *= -1;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        yDelta /= 60;
        zDelta /= 60;
    }

    // Update is called once per frame
    void Update()
    {
        rotateObject();
        movePlatform();
    }
}
