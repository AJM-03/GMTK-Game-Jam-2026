using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public float minLaunchPower, maxLaunchPower;
    public float launchDirection;
    public float moveSpeed;
    private List<Animal> animals = new List<Animal>();
    private AudioSource audioSource;
    private GameController gameController;
    private bool running;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameController = FindObjectOfType<GameController>();
    }

    public void Update()
    {
        if (gameController.gameRunning && !running)
        {
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        running = true;
        yield return new WaitForSeconds(Random.Range(gameController.startingTime / 2.5f, gameController.startingTime /1.5f));
        GetComponent<ParticleSystem>().Play();
        transform.DOMoveX(-transform.position.x, moveSpeed);
        audioSource.Play();
        yield return new WaitForSeconds(moveSpeed);
        audioSource.DOFade(0, 0.3f);
        yield return new WaitForSeconds(0.5f);
        audioSource.Stop();
        GetComponent<ParticleSystem>().Stop();
        //running = false;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent == null) return;
        collision.transform.parent.TryGetComponent<Animal>(out Animal animal);
        if (animal)
        {
            if (!animals.Contains(animal))
            {
                Vector3 offset = new Vector3(
                    Random.Range(-launchDirection, launchDirection),
                    Random.Range(-launchDirection, launchDirection),
                    Random.Range(-launchDirection, launchDirection)
                );

                Vector3 newUp = (transform.up + offset).normalized;
                animal.transform.GetComponent<Rigidbody>().AddForce(newUp * Random.Range(minLaunchPower, maxLaunchPower) * 100 * Time.deltaTime, ForceMode.Impulse);
                animals.Add(animal);
            }
        }
    }
}
