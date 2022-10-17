using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DemoController_N : MonoBehaviour
{

    public AnimationClip AC_demo;
    bool B_CallOnce;

    private void Start()
    {
        GetComponent<Animator>().speed = 0;
        B_CallOnce = true;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == MainController.instance.G_coverPageStart)
        {
            if(B_CallOnce)
            {
                B_CallOnce = false;
                GetComponent<Animator>().speed = 1;
                Invoke("THI_offDemo", AC_demo.length);
            }
           
        }
    }
    void THI_offDemo()
    {
        gameObject.SetActive(false);

        // Debug.Log("Playing demo");
    }
}