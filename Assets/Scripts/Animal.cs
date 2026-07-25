using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AnimalAnimator))]
public class Animal : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animal Info")]
    public string animalName;
    [SerializeField][Range(0.3f, 2.5f)] float walkSpeed = 1.5f;  // How quickly it moves
    public AnimalAnimator.Anim walkingAnimation = AnimalAnimator.Anim.Walk;
    public AnimalAnimator.Anim idleAnimation = AnimalAnimator.Anim.Idle_A;
    public AnimalAnimator.Anim airAnimation = AnimalAnimator.Anim.Lay;
    public List<AnimalAnimator.Emotion> emotions = new List<AnimalAnimator.Emotion>();
    [Range(0f, 0.2f)] public float scaleVariance = 0.1f;  // How much the size of the animal will vary
    public float spacing = 1;  // How far other animals should be from it


    [Header("Movement")]
    [SerializeField] float pushForce;  // How animals get pushed away from eachother
    [SerializeField] LayerMask animalLayer;
    private bool canJump = false;
    private int moveDir;
    [HideInInspector] public bool paired;
    [HideInInspector] public bool walkFowards;
    [SerializeField] PhysicMaterial physicMaterial;
    private float flipMovementTimer;
    [HideInInspector] public bool isGrounded;


    private GameController controller;
    private Rigidbody rb;


    public void SpawnAnimal(GameController c)
    {
        controller = c;
        rb = GetComponent<Rigidbody>();
        SetupCollider();
    }

    
    void Update()
    {
        if (!paired)
        {
            bool castHit = !CastMovement(transform.forward);
            if (Vector3.Distance(transform.position, controller.moveTowardsLocation.position) > spacing && !castHit)
            {
                Vector3 dir = controller.moveTowardsLocation.position - transform.position;
                rb.AddForce((dir / dir.magnitude) * walkSpeed * 100 * Time.deltaTime);
            }

            if (castHit && !Mathf.Approximately(0, transform.position.x))
            {
                if (moveDir != 0)
                {
                    Vector3 dir = moveDir == 1 ? -transform.right : transform.right;
                    bool sideCast = CastMovement(dir);
                    if (!sideCast)  // Cast hit
                    {
                        rb.AddForce((dir / dir.magnitude) * walkSpeed * 50 * Time.deltaTime);
                    }
                    //else  // Cast did not hit
                    //{
                    //    rb.AddForce((-dir / -dir.magnitude) * walkSpeed / 2 * 100 * Time.deltaTime);
                    //}
                }
            }

            //if (!CastMovement(-transform.up))
            //{
            //    rb.AddForce(-transform.up * 1000 * Time.deltaTime);
            //}

            //if (!CastMovement(transform.up) && canJump)
            //{
            //    rb.AddForce(transform.up * 100 * Time.deltaTime, ForceMode.Impulse);
            //    //canJump = false;
            //}

            flipMovementTimer -= Time.deltaTime;
            if (flipMovementTimer <= 0)
            {
                moveDir = Random.Range(0, 3);
                flipMovementTimer = Random.Range(5, 20);
            }

            isGrounded = !(rb.velocity.y > 0.25f || rb.velocity.y < -0.25f);

            //Debug.DrawRay(transform.position, -Vector3.up, Color.blue, spacing);

            transform.LookAt(controller.moveTowardsLocation);
        }
        else if (walkFowards)
        {
            rb.AddForce(Vector3.forward * walkSpeed * 100 * Time.deltaTime);
        }
    }


    private bool CastMovement(Vector3 dir)
    {
        MeshCollider meshCol = GetComponentInChildren<MeshCollider>();
        if (meshCol == null)
        {
            Debug.LogWarning("No Collider found on this GameObject.");
            return false;
        }

        //  Calculate world-space radius (SphereCollider radius * largest scale axis)
        float worldRadius = spacing * Mathf.Max(
            transform.lossyScale.x,
            transform.lossyScale.y,
            transform.lossyScale.z
        );

        //  Start position of the sphere cast
        Vector3 origin = transform.position;

        //  Direction for the sphere cast (example: forward)
        Vector3 direction = dir;

        //  Visualize in Scene view
        Debug.DrawRay(origin, direction * spacing, Color.red);

        //  Perform the sphere cast
        if (Physics.SphereCast(origin, worldRadius, direction, out RaycastHit hit, spacing, animalLayer))
        {
            //Debug.Log($"Hit {hit.collider.name} at distance {hit.distance}");
            return false;
        }

        return true;
    }


    public void OnPointerEnter(PointerEventData eventData) { controller.MouseEnter(this); }
    public void OnPointerExit(PointerEventData eventData) { controller.MouseExit(this); }
    public void OnPointerDown(PointerEventData eventData) { controller.SelectAnimal(this); }


    //private void OnDrawGizmos()
    //{
    //    Vector3 origin = transform.position + transform.rotation * GetComponent<MeshCollider>().center;
    //    float worldRadius = GetComponent<CapsuleCollider>().radius * Mathf.Max(
    //        transform.lossyScale.x,
    //        transform.lossyScale.y,
    //        transform.lossyScale.z
    //    );
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(origin, worldRadius);
    //}

    private void SetupCollider()
    {
        // Try to get the LODGroup component from this GameObject
        LODGroup lodGroup = GetComponent<LODGroup>();
        if (lodGroup == null) return;
        lodGroup.enabled = true;

        // Get all LOD levels
        LOD[] lods = lodGroup.GetLODs();

        // The last LOD is the lowest detail
        LOD lowestLOD = lods[lods.Length - 1];

        // Loop through all renderers in the lowest LOD
        foreach (Renderer renderer in lowestLOD.renderers)
        {
            if (renderer is SkinnedMeshRenderer skinnedRenderer)
            {
                if (skinnedRenderer.sharedMesh != null)
                {
                    //Debug.Log($"Lowest LOD Skinned Mesh: {skinnedRenderer.sharedMesh.name}");

                    MeshCollider collider = skinnedRenderer.gameObject.AddComponent<MeshCollider>();
                    collider.convex = true;
                    collider.providesContacts = true;
                    collider.sharedMaterial = physicMaterial;
                    collider.sharedMesh = skinnedRenderer.sharedMesh; // Triggers automatic baking
                    skinnedRenderer.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                    return;
                }
            }
        }

        Debug.LogError("Mesh not found for collider in " + animalName);
    }
}