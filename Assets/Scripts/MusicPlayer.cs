using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;
    public AudioSource s0, s1, s2;
    public float fadeSpeed;
    public Ease ease;
    [HideInInspector] public int currentIntensity;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        
    }

    public void ChangeIntensity(int intensity)
    {
        if (intensity == currentIntensity) return;
        if (intensity == 0)
        {
            s0.DOFade(1, fadeSpeed).SetEase(ease);
            s1.DOFade(0, fadeSpeed).SetEase(ease);
            s2.DOFade(0, fadeSpeed).SetEase(ease);
        }
        else if (intensity == 1)
        {
            s0.DOFade(0, fadeSpeed).SetEase(ease);
            s1.DOFade(1, fadeSpeed).SetEase(ease);
            s2.DOFade(0, fadeSpeed).SetEase(ease);
        }
        else if (intensity == 2)
        {
            s0.DOFade(0, fadeSpeed).SetEase(ease);
            s1.DOFade(0, fadeSpeed).SetEase(ease);
            s2.DOFade(1, fadeSpeed).SetEase(ease);
        }
        currentIntensity = intensity;
    }
}
