using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public TMP_Text timeText;
    public Slider slider;
    private GameController gameController;
    private bool shaking;
    private Quaternion timerRot;
    private RectTransform timerRect;


    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        timerRect = timeText.GetComponent<RectTransform>();
        timerRot = timerRect.rotation;
    }

    public void Update()
    {
        timeText.text = Mathf.RoundToInt(gameController.timer).ToString();

        float t = Mathf.InverseLerp(gameController.startingTime, 0f, gameController.timer);
        slider.value = t;

        timerRect.rotation = timerRot;

        if (!shaking && gameController.timer <= 5) Shake();
    }

    private void Shake()
    {
        shaking = true;
        slider.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 7f), 0.075f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
    }
}
