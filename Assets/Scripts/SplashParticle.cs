using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashParticle : MonoBehaviour
{
    public float minTime, maxTime;
    private float splashTimer;
    public Vector3 minPos, maxPos;
    private ParticleSystem ps;
    private GameController controller;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
        ps = GetComponent<ParticleSystem>();
    }


    void Update()
    {
        if (!controller.gameRunning) return;
        splashTimer -= Time.deltaTime;
        if (splashTimer <= 0)
        {
            splashTimer = Random.Range(minTime, maxTime);
            transform.localPosition = new Vector3(Random.Range(minPos.x, maxPos.x), transform.localPosition.y, Random.Range(minPos.z, maxPos.z));
            ps.Play();
        }
    }
}
