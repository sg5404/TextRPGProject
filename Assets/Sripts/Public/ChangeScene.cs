using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class ButtonAndScene
{
    public Button ChangeSceneButton;
    public string SceneName;
}

public class ChangeScene : MonoBehaviour
{
    public ButtonAndScene BAS;

    [SerializeField] private Transform BackGroundPannel;


    private void Start()
    {
        BackGroundPannel.localScale = new Vector3(0, 1, 1);
        BackGroundPannel.gameObject.SetActive(true);
        AddChangeSceneEvent();
    }

    void AddChangeSceneEvent()
    {
        BAS.ChangeSceneButton.onClick.AddListener(() => _ChangeScene(BAS.SceneName));
    }

    void _ChangeScene(string SceneName)
    {
        StartCoroutine(Animation(SceneName));
    }

    IEnumerator Animation(string SceneName)
    {
        var seq = DOTween.Sequence();

        seq.Append(BackGroundPannel.DOScale(1f, 1f));
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneName);
    }
}
