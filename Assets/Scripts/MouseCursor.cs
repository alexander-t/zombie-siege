using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    void Start()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(31, 31), CursorMode.Auto);
    }
}
