using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInputReceiver : InputReceiver
{
    [SerializeField] UnityEvent clickEvent;

    public override void OnInputReceived()
    {
        foreach (var item in inputHandlers)
        {
            item.ProcessInput(Input.mousePosition, gameObject, () => clickEvent.Invoke());
        }
    }
}
