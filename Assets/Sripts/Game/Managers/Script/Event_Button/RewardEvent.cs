using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardEvent : Events
{
    public override void EventStart()
    {
        UIManager.Instance.RewardPannelSetActive(true); 
    }
}
