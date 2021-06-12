using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMouseCursor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Texture2D cursorTexture;

    [SerializeField]
    private Vector2 hotSpot = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        // Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
