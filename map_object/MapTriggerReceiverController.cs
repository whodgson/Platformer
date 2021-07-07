using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.script;
using System;

public class MapTriggerReceiverController : MonoBehaviour
{
    public event EventHandler Activate;
    public event EventHandler Deactivate;

    public void OnActivate()
    {
        EventHandler handler = Activate;
        if (handler != null) handler(this, EventArgs.Empty);
    }

    public void OnDeactivate()
    {
        EventHandler handler = Deactivate;
        if (handler != null) handler(this, EventArgs.Empty);
    }
}
