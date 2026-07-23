using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    public TMP_Text timeText;
    public GameController gameController;

    public void Update()
    {
        timeText.text = Mathf.RoundToInt(gameController.timer).ToString();
    }
}
