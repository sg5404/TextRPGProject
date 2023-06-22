using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu_TextManager : MonoBehaviour
{
    private float timer = 0;
    [SerializeField] TextMeshProUGUI TouchText;

    private void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime * 2;
        float opacity = 0.2f + Mathf.Sin(timer);
        //opacity = Mathf.Clamp(opacity, 0f, 1f) + 1;
        TouchText.color = new Color(1, 1, 1, opacity + 1);
        TouchText.outlineColor = new Color(0, 0, 0, opacity + 1);
    }
}
