using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float startingTime;
    [SerializeField] int targetNumberOfAnimals;
    public Transform moveTowardsLocation;
    [SerializeField] Transform teleportLocation;
    [SerializeField] List<GameObject> animalPrefabs = new List<GameObject>();
    [SerializeField] Transform spawnPosition;
    [SerializeField] LayerMask barrierLayer;
    [SerializeField] ParticleSystem puffParticles;
    [SerializeField] ParticleSystem poofParticles;
    [SerializeField] List<AudioClip> poofSounds = new List<AudioClip>();
    [SerializeField] AudioSource poofAudioSource;
    [SerializeField] AudioSource pairAudioSource;

    public float timer;
    public int score;

    public List<Animal> animals = new List<Animal>();
    private Animal highlightedAnimal;
    private Animal selectedAnimal;
    private int possiblePairs;
    private bool canSelect = true;

    void Start()
    {
        StartGame();
    }


    void Update()
    {
        timer -=Time.deltaTime;
    }


    public void StartGame()
    {
        timer = startingTime;
        StartCoroutine(SpawnAnimals(targetNumberOfAnimals));
    }

    public void EndGame()
    {

    }

    private IEnumerator SpawnAnimals(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            SpawnAnimal();
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void SpawnAnimal()
    {
        GameObject animalType;
        do
        {
            animalType = animalPrefabs[UnityEngine.Random.Range(0, animalPrefabs.Count)];
        } while (animalType == null);


        GameObject newAnimal = Instantiate(animalType,
                                           spawnPosition.position + new Vector3(UnityEngine.Random.Range(-spawnPosition.localScale.x / 2, spawnPosition.localScale.x / 2), 0, UnityEngine.Random.Range(-spawnPosition.localScale.z / 2, -spawnPosition.localScale.z / 2)),
                                           Quaternion.identity);
        Animal animalScript = newAnimal.GetComponent<Animal>();
        newAnimal.transform.localScale *= UnityEngine.Random.Range(1f - animalScript.scaleVariance, 1f + animalScript.scaleVariance);


        foreach(Animal a in animals)
        {
            if (a.animalName == animalScript.animalName && !a.canBePaired)
            {
                possiblePairs++;
                a.canBePaired = true;
                animalScript.canBePaired = true;
            }
        }
        animals.Add(animalScript);
        animalScript.SpawnAnimal(this);
    }

    public void SelectAnimal(Animal a)
    {
        if (a == selectedAnimal || a != highlightedAnimal || !canSelect) return;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Spin);
        SwapLayer(a.gameObject, "Selected");
        poofParticles.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        poofParticles.Play();

        if (!selectedAnimal)
        {
            selectedAnimal = highlightedAnimal;
        }
        else
        {
            if (PairAnimals(a, selectedAnimal)) return;
            SwapLayer(selectedAnimal.gameObject, "Animal");
            selectedAnimal = a;
        }
        int randomIndex = UnityEngine.Random.Range(0, poofSounds.Count);
        poofAudioSource.PlayOneShot(poofSounds[randomIndex]);
    }

    public void MouseEnter(Animal a)
    {
        if (highlightedAnimal != a) highlightedAnimal = a;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Clicked);
        if (selectedAnimal != a) SwapLayer(a.gameObject, "Highlighted");
    }

    public void MouseExit(Animal a)
    {
        if (highlightedAnimal == a) highlightedAnimal = null;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Idle_A);
        if (selectedAnimal != a) SwapLayer(a.gameObject, "Animal");
    }

    private bool PairAnimals(Animal a, Animal b)
    {
        if (a.animalName == b.animalName && a != b)
        {
            score += 2;
            pairAudioSource.Play();
            highlightedAnimal = null;
            selectedAnimal = null;
            StartCoroutine(MovePair(a, b));
            StartCoroutine(SpawnAnimals(2));
            return true;
        }
        return false;
    }

    private IEnumerator MovePair(Animal a, Animal b)
    {
        canSelect = false;
        yield return new WaitForSeconds(0.5f);
        canSelect = true;

        a.gameObject.SetActive(false);
        puffParticles.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        a.transform.position = teleportLocation.position + new Vector3(-a.spacing / 2, 0, 0);
        a.GetComponentInChildren<MeshCollider>().excludeLayers = barrierLayer;

        yield return 0;

        b.gameObject.SetActive(false);
        puffParticles.transform.position = b.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        b.transform.position = teleportLocation.position + new Vector3(a.spacing / 2, 0, 0);
        b.GetComponentInChildren<MeshCollider>().excludeLayers = barrierLayer;

        yield return new WaitForSeconds(0.3f);

        a.gameObject.SetActive(true);
        a.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        puffParticles.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        a.paired = true;
        SwapLayer(a.gameObject, "Default");

        yield return 0;

        b.gameObject.SetActive(true);
        b.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        puffParticles.transform.position = b.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        b.paired = true;
        b.gameObject.layer = 0;
        SwapLayer(b.gameObject, "Default");


        yield return new WaitForSeconds(0.25f);
        a.walkFowards = true;
        b.walkFowards = true;

        yield return new WaitForSeconds(3);
        Destroy(a.gameObject);
        Destroy(b.gameObject);
    }


    public void SwapLayer(GameObject obj, string layerName)
    {
        obj.layer = LayerMask.NameToLayer(layerName);
        foreach(Transform t in obj.transform)
        {
            SwapLayer(t.gameObject, layerName);
        }
    }
}
