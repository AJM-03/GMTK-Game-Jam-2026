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


    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void Update()
    {
        timeText.text = Mathf.RoundToInt(gameController.timer).ToString();

        float t = Mathf.InverseLerp(gameController.startingTime, 0f, gameController.timer);
        slider.value = t;
    }
}
