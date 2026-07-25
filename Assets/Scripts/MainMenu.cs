using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup menuCanvasGroup, optionsCanvasGroup;

    public void InitMenu()
    {
        menuCanvasGroup.alpha = 1f;
        menuCanvasGroup.interactable = true;

        optionsCanvasGroup.alpha = 0f;
        optionsCanvasGroup.interactable = false;
        optionsCanvasGroup.blocksRaycasts = false;
    }

    public void PlayButton()
    {
        StartCoroutine(FindObjectOfType<GameController>().StartGame());
    }

    public void OptionsButton()
    {
        optionsCanvasGroup.alpha = 0f;
        optionsCanvasGroup.interactable = true;
        optionsCanvasGroup.blocksRaycasts = true;
        optionsCanvasGroup.DOFade(1, 0.25f).SetEase(Ease.OutSine);

        menuCanvasGroup.alpha = 1f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
        menuCanvasGroup.DOFade(0, 0.25f).SetEase(Ease.InSine);

        GetComponent<AudioSource>().Play();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MenuButton()
    {
        optionsCanvasGroup.alpha = 1f;
        optionsCanvasGroup.interactable = false;
        optionsCanvasGroup.blocksRaycasts = false;
        optionsCanvasGroup.DOFade(0, 0.25f).SetEase(Ease.InSine);

        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
        menuCanvasGroup.DOFade(1, 0.25f).SetEase(Ease.OutSine);

        GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        if (menuCanvasGroup.interactable && Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
