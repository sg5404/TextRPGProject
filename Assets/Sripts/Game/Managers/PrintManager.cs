using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrintManager : MonoSingleton<PrintManager>
{
    [SerializeField] private TextMeshProUGUI SummaryText;
    [SerializeField] private Image SummaryImage;
    [SerializeField] private List<Button> SelectButtons;
    [SerializeField] private List<TextMeshProUGUI> SelectButtons_Texts;

    [SerializeField] private float printingTime;

    private void Start()
    {

    }

    internal void SetStage(string text, Sprite sprite, int ButtonNum, List<OptionTextAndKind> Strings)
    {
        SummaryText.text = null;
        SummaryImage.sprite = sprite;
        SummaryImage.color = new Color(SummaryImage.color.r, SummaryImage.color.g, SummaryImage.color.b, 0f);
        
        for(int num = 0; num < SelectButtons.Count; num++)
        {
            var ButtonColor = SelectButtons[num].GetComponent<Image>().color;
            var TextColor = SelectButtons_Texts[num].color;

            SelectButtons[num].GetComponent<Image>().color = new Color(ButtonColor.r, ButtonColor.g, ButtonColor.b, 0f);
            SelectButtons_Texts[num].color = new Color(TextColor.r, TextColor.g, TextColor.b, 0f);

            SelectButtons[num].interactable = false;

        }

        StartCoroutine(SummaryPrint(text, sprite, ButtonNum, Strings));
    }

    private IEnumerator SummaryPrint(string text, Sprite sprite, int ButtonNum, List<OptionTextAndKind> Strings)
    {
        int TextNum = 0;
        int NowButton = 0;
        bool isFirst = true;

        while(true)
        {
            if (TextNum < text.Length) SummaryText.text += text[TextNum++]; //텍스트 출력하는 부분
            else if (SummaryImage.color.a < 0.96f) //이미지 페이드 인 하는 부분
            {
                Color color = SummaryImage.color;
                color.a += 0.025f;
                SummaryImage.color = color;
            }
            else if(SelectButtons[NowButton].GetComponent<Image>().color.a < 0.96f) //버튼들 페이드 인 하는 부분
            {
                if (isFirst)
                {
                    SelectButtons_Texts[NowButton].text = Strings[NowButton].OptionText;
                    isFirst = false;
                }

                Color Buttoncolor = SelectButtons[NowButton].GetComponent<Image>().color;
                Buttoncolor.a += 0.03f;
                SelectButtons[NowButton].GetComponent<Image>().color = Buttoncolor;

                Color TextColor = SelectButtons_Texts[NowButton].color;
                TextColor.a += 0.05f;
                SelectButtons_Texts[NowButton].color = TextColor;

                if(Buttoncolor.a > 0.6f)
                {
                    SelectButtons[NowButton].interactable = true;
                    isFirst = true;

                    if (ButtonNum == NowButton + 1) break;
                    else NowButton++;
                }
            }

            yield return new WaitForSeconds(printingTime);
        }
    }
}
