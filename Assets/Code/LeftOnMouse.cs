using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class LeftOnMouse : PlayerMovement, IPointerDownHandler, IPointerUpHandler
{
    public Transform tranform;
    public bool buttonHeld2;

    void Start()
    {

    }

    void Update()
    {



    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonHeld2 = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeld2 = false;
        
    }
}
