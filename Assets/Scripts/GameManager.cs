using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            Screen.SetResolution(1024, 768, FullScreenMode.Windowed);
        }
        else if (Input.GetKey(KeyCode.F2))
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
        else if (Input.GetKey(KeyCode.F3))
        {
            Screen.SetResolution(2560, 1440, FullScreenMode.Windowed);
        }
    }
}
