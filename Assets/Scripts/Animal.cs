using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimalAnimator))]
public class Animal : MonoBehaviour
{
    [Header("Animal Info")]
    [SerializeField] float walkSpeed;  // How quickly it moves
    public AnimalAnimator.Anim walkingAnimation = AnimalAnimator.Anim.Walk;
    public AnimalAnimator.Anim idleAnimation = AnimalAnimator.Anim.Idle_A;
    [SerializeField] float spacing;  // How far other animals should be from it
    public float scaleVariance;  // How much the size of the animal will vary


    [Header("Movement")]
    [SerializeField] float pushForce;  // How animals get pushed away from eachother


    private GameController controller;
    private Rigidbody rb;


    public void SpawnAnimal(GameController c)
    {
        controller = c;
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        for (int i = 0; i < controller.animals.Count; i++)  // Move away from other enemies
        {
            if (controller.animals[i] != null && controller.animals[i].transform != transform && Vector3.Distance(transform.position, controller.animals[i].transform.position) < spacing)
            {
                Vector3 dir = controller.moveTowardsLocation.position - transform.position;
                rb.AddForce((dir / dir.magnitude) * -pushForce * 100 * Time.deltaTime / Vector3.Distance(transform.position, controller.animals[i].transform.position));
            }
        }

        if (Vector3.Distance(transform.position, controller.moveTowardsLocation.position) > spacing)
        {
            Vector3 dir = controller.moveTowardsLocation.position - transform.position;
            rb.AddForce((dir / dir.magnitude) * walkSpeed * 100 * Time.deltaTime);
        }

        transform.LookAt(controller.moveTowardsLocation);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spacing);
    }
}
