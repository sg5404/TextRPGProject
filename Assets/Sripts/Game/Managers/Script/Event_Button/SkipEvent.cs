using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipEvent : Events
{
    public override void EventStart()
    {
        StageManager.Instance.NextStage();
    }
}
