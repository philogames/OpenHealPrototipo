using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    KeyCode speedKey = KeyCode.Alpha2;
#if UNITY_EDITOR

    private void OnEnable()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(speedKey))
            Time.timeScale = 5f;

        if (Input.GetKeyUp(speedKey))
        {
            Time.timeScale = 1f;
        }
       
    }
#endif
}
