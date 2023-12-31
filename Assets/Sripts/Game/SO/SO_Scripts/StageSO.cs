using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "SO/StageSO")]
public class StageSO : ScriptableObject
{
    public Sprite StageSprite;
    [Multiline(5)]
    public string StageSummary;
    public int OptionCount;
    public List<OptionTextAndKind> Options;
    public bool isRandomStage;
}

[System.Serializable]
public class OptionTextAndKind
{
    public string OptionText;
    public EventKinds EventKind;
    public Events LinkedEvent; 
}

public enum EventKinds
{
    None,
    Battle,
    Reward,
    Skip,
    Reward_First,
    Random,
}
