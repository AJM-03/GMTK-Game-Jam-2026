using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float startingTime;
    public Transform moveTowardsLocation;
    [SerializeField] Transform teleportLocation;
    [SerializeField] List<GameObject> animalPrefabs = new List<GameObject>();
    [SerializeField] LayerMask barrierLayer;
    [SerializeField] ParticleSystem puffParticles;
    [SerializeField] ParticleSystem poofParticles;


    public float timer;
    public float score;

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
        SpawnAnimals(50);
    }

    public void EndGame()
    {

    }

    private void SpawnAnimals(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            SpawnAnimal();
        }
    }

    private void SpawnAnimal()
    {
        GameObject newAnimal = Instantiate(animalPrefabs[Random.Range(0, animalPrefabs.Count)],
                                           new Vector3(Random.Range(-15, 15), 0.5f, Random.Range(-20, -5)),
                                           Quaternion.identity);
        Animal animalScript = newAnimal.GetComponent<Animal>();
        newAnimal.transform.localScale *= Random.Range(1f - animalScript.scaleVariance, 1f + animalScript.scaleVariance);
        animalScript.SpawnAnimal(this);
        animals.Add(animalScript);
    }

    public void SelectAnimal(Animal a)
    {
        if (a != highlightedAnimal || !canSelect) return;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Spin);
        poofParticles.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        poofParticles.Play();
        if (!selectedAnimal)
        {
            selectedAnimal = highlightedAnimal;
        }
        else
        {
            if (PairAnimals(a, selectedAnimal)) return;

            selectedAnimal = a;
        }
    }

    public void MouseEnter(Animal a)
    {
        if (highlightedAnimal != a) highlightedAnimal = a;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Clicked);

    }

    public void MouseExit(Animal a)
    {
        if (highlightedAnimal == a) highlightedAnimal = null;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Idle_A);

    }

    private bool PairAnimals(Animal a, Animal b)
    {
        if (a.animalName == b.animalName && a != b)
        {
            score += 2;
            highlightedAnimal = null;
            selectedAnimal = null;
            StartCoroutine(MovePair(a, b));
            return true;
        }
        return false;
    }

    private IEnumerator MovePair(Animal a, Animal b)
    {
        canSelect = false;
        yield return new WaitForSeconds(0.5f);

        a.gameObject.SetActive(false);
        puffParticles.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        a.transform.position = teleportLocation.position + new Vector3(-a.spacing / 2, 0, 0);
        a.GetComponent<CapsuleCollider>().excludeLayers = barrierLayer;

        yield return 0;

        b.gameObject.SetActive(false);
        puffParticles.transform.position = b.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        b.transform.position = teleportLocation.position + new Vector3(a.spacing / 2, 0, 0);
        b.GetComponent<CapsuleCollider>().excludeLayers = barrierLayer;

        yield return new WaitForSeconds(0.3f);
        canSelect = true;

        a.gameObject.SetActive(true);
        a.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        puffParticles.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        a.paired = true;
        a.gameObject.layer = 0;

        yield return 0;

        b.gameObject.SetActive(true);
        b.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        puffParticles.transform.position = b.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        b.paired = true;
        b.gameObject.layer = 0;


        yield return new WaitForSeconds(0.25f);
        a.walkFowards = true;
        b.walkFowards = true;

        yield return new WaitForSeconds(3);
        Destroy(a.gameObject);
        Destroy(b.gameObject);
    }
}
