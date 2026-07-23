using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public GameController gameController;

    public void Update()
    {
        scoreText.text = gameController.score.ToString();
    }
}
