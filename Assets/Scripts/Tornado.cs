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
    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(Random.Range(25, 50));
        transform.DOMoveX(-transform.position.x, moveSpeed);
        audio.Play();
        yield return new WaitForSeconds(moveSpeed);
        audio.DOFade(0, 0.3f);
        yield return new WaitForSeconds(0.5f);
        audio.Stop();
    }

    public void OnTriggerEnter(Collider collision)
    {
        Animal animal;
        if (collision.transform.parent.TryGetComponent<Animal>(out animal) && !animals.Contains(animal))
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
