using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(AnimalAnimator))]
public class Animal : MonoBehaviour
{
    [Header("Animal Info")]
    [SerializeField][Range(0.3f, 2.5f)] float walkSpeed = 1.5f;  // How quickly it moves
    public AnimalAnimator.Anim walkingAnimation = AnimalAnimator.Anim.Walk;
    public AnimalAnimator.Anim idleAnimation = AnimalAnimator.Anim.Idle_A;
    [Range(0f, 0.2f)] public float scaleVariance = 0.1f;  // How much the size of the animal will vary
    [SerializeField] float spacing = 1;  // How far other animals should be from it


    [Header("Movement")]
    [SerializeField] float pushForce;  // How animals get pushed away from eachother
    [SerializeField] LayerMask animalLayer;
    private bool canJump = false;
    private bool movingIn;


    private GameController controller;
    private Rigidbody rb;


    public void SpawnAnimal(GameController c)
    {
        controller = c;
        rb = GetComponent<Rigidbody>();
        movingIn = Random.Range(0, 2) == 1;
    }

    
    void Update()
    {
        //for (int i = 0; i < controller.animals.Count; i++)  // Move away from other enemies
        //{
        //    if (controller.animals[i] != null && controller.animals[i].transform != transform && Vector3.Distance(transform.position, controller.animals[i].transform.position) < spacing)
        //    {
        //        Vector3 dir = controller.moveTowardsLocation.position - transform.position;
        //        rb.AddForce((dir / dir.magnitude) * -pushForce * 100 * Time.deltaTime / Vector3.Distance(transform.position, controller.animals[i].transform.position));
        //    }
        //}

        bool castHit = !CastMovement(transform.forward);
        if (Vector3.Distance(transform.position, controller.moveTowardsLocation.position) > spacing && !castHit)
        {
            Vector3 dir = controller.moveTowardsLocation.position - transform.position;
            rb.AddForce((dir / dir.magnitude) * walkSpeed * 100 * Time.deltaTime);
        }

        if (castHit && !Mathf.Approximately(0, transform.position.x))
        {
            bool moveLeft = transform.position.x > 0;
            if (movingIn) moveLeft = !moveLeft;
            Vector3 dir = moveLeft ? -transform.right : transform.right;
            bool sideCast = CastMovement(dir);
            if (sideCast)
            {
                rb.AddForce((dir / dir.magnitude) * walkSpeed *.75f * 100 * Time.deltaTime);
            }
        }

        if (!CastMovement(-transform.up))
        {
            rb.AddForce(-transform.up * 1000 * Time.deltaTime);
        }

        if (!CastMovement(transform.up) && canJump)
        {
            rb.AddForce(transform.up * 100 * Time.deltaTime, ForceMode.Impulse);
            //canJump = false;
        }

        transform.LookAt(controller.moveTowardsLocation);
    }


    private bool CastMovement(Vector3 dir)
    {
        CapsuleCollider capsuleCol = GetComponent<CapsuleCollider>();
        if (capsuleCol == null)
        {
            Debug.LogWarning("No SphereCollider found on this GameObject.");
            return false;
        }

        //  Calculate world-space radius (SphereCollider radius * largest scale axis)
        float worldRadius = capsuleCol.radius * Mathf.Max(
            transform.lossyScale.x,
            transform.lossyScale.y,
            transform.lossyScale.z
        );

        //  Start position of the sphere cast
        Vector3 origin = transform.position + transform.rotation * capsuleCol.center;

        //  Direction for the sphere cast (example: forward)
        Vector3 direction = dir;

        //  Visualize in Scene view
        Debug.DrawRay(origin, direction * spacing, Color.red);

        //  Perform the sphere cast
        if (Physics.SphereCast(origin, worldRadius, direction, out RaycastHit hit, spacing, animalLayer))
        {
            Debug.Log($"Hit {hit.collider.name} at distance {hit.distance}");
            return false;
        }

        return true;
    }


    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + transform.rotation * GetComponent<CapsuleCollider>().center;
        float worldRadius = GetComponent<CapsuleCollider>().radius * Mathf.Max(
            transform.lossyScale.x,
            transform.lossyScale.y,
            transform.lossyScale.z
        );
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, worldRadius);
    }
}
