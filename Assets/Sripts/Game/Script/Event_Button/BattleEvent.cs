using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvent : Events
{
    public override void EventStart()
    {
        UIManager.Instance.BattlePannelSetActive(true);
    }
}
