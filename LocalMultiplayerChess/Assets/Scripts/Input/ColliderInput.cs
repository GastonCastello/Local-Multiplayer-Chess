using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInput : InputReceiver
{
    Vector3 clickPos;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && (GameManager.instance.camBlack.activeInHierarchy || GameManager.instance.camWhite.activeInHierarchy))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                clickPos = hit.point;
                OnInputReceived();
            }
        }
    }

    public override void OnInputReceived()
    {
        foreach (var item in inputHandlers)
        {
            item.ProcessInput(clickPos, null, null);
        }
    }
}
