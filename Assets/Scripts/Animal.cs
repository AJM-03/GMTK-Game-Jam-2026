using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Animal Info")]
    [SerializeField] float walkSpeed;  // How quickly it moves
    public AnimalAnimator.Anim walkingAnimation = AnimalAnimator.Anim.Walk;
    public AnimalAnimator.Anim idleAnimation = AnimalAnimator.Anim.Idle_A;
    [SerializeField] float spacing;  // How far other animals should be from it


    [Header("Movement")]
    [SerializeField] float pushForce;  // How animals get pushed away from eachother


    private GameController controller;

    void Start()
    {

    }

    public void SpawnAnimal(GameController c)
    {
        controller = c;
    }

    
    void Update()
    {
        for (int i = 0; i < controller.animals.Count; i++)  // Move away from other enemies
        {
            if (controller.animals[i] != null && controller.animals[i].transform != transform && Vector3.Distance(transform.position, controller.animals[i].transform.position) < spacing)
            {
                transform.position = Vector3.MoveTowards(transform.position, controller.animals[i].transform.position, -walkSpeed * Time.deltaTime);
            }
        }

        if (Vector3.Distance(transform.position, controller.moveTowardsLocation.position) > spacing * 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, controller.moveTowardsLocation.position, walkSpeed * Time.deltaTime);
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, controller.moveTowardsLocation.position, -walkSpeed * Time.deltaTime);

        transform.LookAt(controller.moveTowardsLocation);
    }
}
