using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardButtonSetting : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Summary;
    public Image StatImage;

    public void Setting(string _Name, string _Summary, Sprite _StatImage)
    {
        Name.text = _Name;
        Summary.text = _Summary;
        StatImage.sprite = _StatImage;
    }
}
