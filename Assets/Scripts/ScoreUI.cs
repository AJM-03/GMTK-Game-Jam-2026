using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    private GameController gameController;
    private int lastScore;
    bool rotateRight;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void Update()
    {
        scoreText.text = gameController.score.ToString();
        if (gameController.score != lastScore)
        {
            Shake();
            lastScore = gameController.score;
            rotateRight = !rotateRight;
        }
    }

    public void Shake()
    {
        scoreText.transform.parent.GetComponent<RectTransform>().DOPunchRotation(new Vector3(0, 0, rotateRight ? 20f : -20f), 0.15f);
        scoreText.transform.parent.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.15f);
    }
}
