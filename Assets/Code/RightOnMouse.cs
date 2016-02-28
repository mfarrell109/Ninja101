using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class RightOnMouse : PlayerMovement, IPointerDownHandler, IPointerUpHandler
{
    public Transform tranform;
    public bool buttonHeld;
    
    void Start()
    {
       
    }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonHeld = true;
        
        //throw new NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeld = false;

        //throw new NotImplementedException();
    }
}

