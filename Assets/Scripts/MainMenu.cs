using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup menuCanvasGroup, optionsCanvasGroup;
    public TMP_Text highscoreText;
    [HideInInspector] int highscore;

    public void InitMenu()
    {
        menuCanvasGroup.alpha = 1f;
        menuCanvasGroup.interactable = true;

        optionsCanvasGroup.alpha = 0f;
        optionsCanvasGroup.interactable = false;
        optionsCanvasGroup.blocksRaycasts = false;

        highscoreText.transform.parent.gameObject.SetActive(false);
        if (PlayerPrefs.HasKey("Highscore")) highscore = PlayerPrefs.GetInt("Highscore");
        if (highscore != 0)
        {
            highscoreText.transform.parent.gameObject.SetActive(true);
            highscoreText.text = highscore.ToString();
        }
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
