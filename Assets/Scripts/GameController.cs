using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] float startingTime;
    public Transform moveTowardsLocation;
    [SerializeField] Transform teleportLocation;
    [SerializeField] List<GameObject> animalPrefabs = new List<GameObject>();


    public float timer;
    public float score;

    public List<Animal> animals = new List<Animal>();
    private Animal highlightedAnimal;
    private Animal selectedAnimal;
    private int possiblePairs;



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
                                           new Vector3(Random.Range(-15, 15), 0, Random.Range(-20, -5)),
                                           Quaternion.identity);
        Animal animalScript = newAnimal.GetComponent<Animal>();
        newAnimal.transform.localScale *= Random.Range(1f - animalScript.scaleVariance, 1f + animalScript.scaleVariance);
        animalScript.SpawnAnimal(this);
        animals.Add(animalScript);
    }

    public void SelectAnimal(Animal a)
    {
        if (a != highlightedAnimal) return;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Spin);
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
        if (a.animalName == b.animalName)
        {
            score += 2;
            Destroy(a.gameObject);
            Destroy(b.gameObject);
            highlightedAnimal = null;
            selectedAnimal = null;
            return true;
        }
        return false;
    }
}
