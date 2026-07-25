using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimalAnimator;


public class AnimalAnimator : MonoBehaviour
{
    private Animator animator;
    private Animal animal;
    private bool playingOneShot;
    private Coroutine killCo, fadeCo;
    private float crossfadeLength;
    private string fadingTo;

    public enum Anim
    {
        Attack,
        Bounce,
        Clicked,
        Death,
        Eat,
        Fear,
        Fly,
        Hit,
        Idle_A, 
        Idle_B, 
        Idle_C,
        Jump,
        Roll,
        Lay,
        Run,
        Sit,
        Spin,
        Swim,
        Walk
    }

    public enum Emotion
    {      
        Eyes_Annoyed,
        Eyes_Blink,
        Eyes_Cry,
        Eyes_Dead,
        Eyes_Excited,
        Eyes_Happy,
        Eyes_LookDown,
        Eyes_LookIn,
        Eyes_LookOut,
        Eyes_LookUp,
        Eyes_Rabid,
        Eyes_Sad,
        Eyes_Shrink,
        Eyes_Sleep,
        Eyes_Spin,
        Eyes_Squint,
        Eyes_Trauma,
        Sweat_L,
        Sweat_R,
        Teardrop_L,
        Teardrop_R
    };

    void Start()
    {
        animator = GetComponent<Animator>();
        animal = GetComponent<Animal>();

        ChangeAnimation(animal.idleAnimation);
        ChangeShapekey(Emotion.Eyes_Blink);  // Start by blinking
    }

    private void Update()
    {
        if (!animal.isGrounded)
            ChangeAnimation(animal.airAnimation);
        else if (GetComponent<Rigidbody>().velocity.magnitude > .5f)
            ChangeAnimation(animal.walkingAnimation);
        else
            ChangeAnimation(animal.idleAnimation);
    }


    public void ChangeAnimation(Anim a)
    {



        if (a == Anim.Spin)  // If Spin/Splash animation
        {
            if (animator.HasState(0, Animator.StringToHash("Spin")))
            {
                StartCoroutine(PlayOneShot("Spin"));
                ChangeShapekey(Emotion.Eyes_Spin);
            }
            else if (animator.HasState(0, Animator.StringToHash("Splash")))
            {
                StartCoroutine(PlayOneShot("Splash"));
            }
        }
        else if (a == Anim.Clicked)
        {
            StartCoroutine(PlayOneShot(Anim.Clicked.ToString()));
            ChangeShapekey(Emotion.Eyes_Happy);
        }
        else
        {
            if (playingOneShot) return;
            if (IsAnimationPlaying(a.ToString())) return;

            PlayAnim(a.ToString());
        }
    }

    public void ChangeShapekey(Emotion e)
    {
        animator.Play(e.ToString());
    }

    bool IsAnimationPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
    }

    private IEnumerator PlayOneShot(string animation, bool restart = true, float time = 0.417f)
    {
        if (!restart && IsAnimationPlaying(animation)) yield break;
        if (KillOneShot()) yield return 0;
        killCo = StartCoroutine(OneShot(animation, time));
    }

    private IEnumerator OneShot(string animation, float time)
    {
        playingOneShot = true;
        PlayAnim(animation);
        yield return new WaitForSeconds(time);
        playingOneShot = false;
    }

    private bool KillOneShot()
    {
        if (killCo == null) return false;
        StopCoroutine(killCo);
        PlayAnim(Anim.Idle_A.ToString());
        killCo = null;
        playingOneShot = false;
        return true;
    }

    private void PlayAnim(string a)
    {
        if (fadingTo == a) return;
        if (!animator.HasState(0, Animator.StringToHash(a))) return;

        int emotion = Random.Range(0, animal.emotions.Count);
        ChangeShapekey(animal.emotions[emotion]);

        fadeCo = StartCoroutine(FadeAnim(a));
    }

    private IEnumerator FadeAnim(string a)
    {
        fadingTo = a;
        animator.CrossFade(a.ToString(), crossfadeLength);
        yield return new WaitForSeconds(crossfadeLength);
    }

    private bool KillFade()
    {
        if (fadeCo == null) return false;
        StopCoroutine(fadeCo);
        fadeCo = null;
        fadingTo = "";
        return true;
    }
}
