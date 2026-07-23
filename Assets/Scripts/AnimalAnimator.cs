using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalAnimator : MonoBehaviour
{
    private Animator animator;
    private Animal animal;

    private Vector3 prevPosition;

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
        Idle_B, Idle_C,
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
        if (GetComponent<Rigidbody>().velocity.magnitude > 1)
            ChangeAnimation(animal.walkingAnimation);
        else
            ChangeAnimation(animal.idleAnimation);

        //prevPosition = transform.position;
    }


    public void ChangeAnimation(Anim a)
    {
        // If Spin/Splash animation
        if ((int)a == 16)
        {
            if (animator.HasState(0, Animator.StringToHash("Spin")))
            {
                animator.Play("Spin");
                // dropdownAnimation.options[index] = new Dropdown.OptionData("Spin");
            }
            else if (animator.HasState(0, Animator.StringToHash("Splash")))
            {
                animator.Play("Splash");
                // dropdownAnimation.options[index] = new Dropdown.OptionData("Splash");
            }
        }
        else
        {
            animator.Play(a.ToString());
        }
    }

    public void ChangeShapekey(Emotion e)
    {
        animator.Play(e.ToString());
    }
}
