using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaterLevel : MonoBehaviour
{
    public float maxHeight, minHeight, animateHeight, animateSpeed;
    private GameController controller;
    private Transform parent;

    void Start()
    {
        controller = FindObjectOfType<GameController>();
        parent = transform.parent;
        parent.DOLocalMoveY(animateHeight, animateSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    void Update()
    {
        if (!controller) return;

        float t = Mathf.InverseLerp(controller.startingTime, 0f, controller.timer);
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(minHeight, maxHeight, t), transform.localPosition.z);
    }
}
