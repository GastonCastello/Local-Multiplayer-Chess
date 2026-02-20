using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : Button
{
    InputReceiver receiver;

    protected override void Awake()
    {
        base.Awake();
        receiver = GetComponent<UIInputReceiver>();
        onClick.AddListener(() => receiver.OnInputReceived());
    }
}
