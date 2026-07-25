using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float startingTime;
    [SerializeField] int targetNumberOfAnimals;
    [SerializeField] int targetNumberOfPairs;
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

    [Header("Menu")]
    [SerializeField] CanvasGroup mainMenuCanvas;
    [SerializeField] CanvasGroup HUDCanvas;
    [SerializeField] CanvasGroup ThunderCanvas;
    [SerializeField] CanvasGroup FadeCanvas;
    [SerializeField] ParticleSystem rainParticles;
    [SerializeField] Transform menuAnimalPosition;
    [HideInInspector] public Animal menuAnimal;

    public float timer;
    public int score;
    [HideInInspector] public bool gameRunning;

    public List<Animal> animals = new List<Animal>();
    private Animal highlightedAnimal;
    private Animal selectedAnimal;
    private int possiblePairs;
    private bool canSelect = true;

    void Start()
    {
        gameRunning = false;
        timer = startingTime;
        HUDCanvas.alpha = 0f;
        ThunderCanvas.alpha = 0f;
        FadeCanvas.alpha = 1f;
        mainMenuCanvas.alpha = 1f;
        mainMenuCanvas.interactable = true;
        menuAnimal = SpawnAnimal();
        menuAnimal.transform.position = menuAnimalPosition.position;
        menuAnimal.transform.rotation = menuAnimalPosition.rotation;
        FadeCanvas.DOFade(0, 2f).SetEase(Ease.OutSine);
    }


    void Update()
    {
        if (gameRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                StartCoroutine(EndGame());
        }
    }


    public IEnumerator StartGame()
    {
        mainMenuCanvas.DOFade(0, 0.2f).SetEase(Ease.InSine);
        mainMenuCanvas.interactable = false;
        mainMenuCanvas.blocksRaycasts = false;

        ThunderCanvas.DOFade(1, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuart);
        ThunderCanvas.GetComponent<AudioSource>().Play();

        menuAnimal.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Jump, true);

        yield return new WaitForSeconds(0.417f);

        menuAnimal.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Fear);
        rainParticles.Play();
        rainParticles.GetComponent<AudioSource>().Play();


        yield return new WaitForSeconds(3);

        StartCoroutine(SpawnAnimals(targetNumberOfAnimals));
        gameRunning = true;

        yield return new WaitForSeconds(2.5f);
        HUDCanvas.DOFade(1, 0.6f).SetEase(Ease.OutSine);
    }

    public IEnumerator EndGame()
    {
        gameRunning = false;

        if (highlightedAnimal != null) highlightedAnimal.outlineColour = 0;
        if (selectedAnimal != null) selectedAnimal.outlineColour = 0;

        foreach (Animal a in animals)
        {
            if (a != null)
            {
                a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Fear);
                a.GetComponent<AnimalAnimator>().ChangeShapekey(AnimalAnimator.Emotion.Eyes_Shrink);
            }
        }
        yield return new WaitForSeconds(5f);
        Camera.main.transform.DOMove(new Vector3(3.5f, 2, 10), 6f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(3f);
        FadeCanvas.DOFade(1, 2.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator SpawnAnimals(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            SpawnAnimal();
            yield return new WaitForSeconds(0.1f);
        }
    }


    private Animal SpawnAnimal()
    {
        GameObject animalType;
        do
        {
            animalType = animalPrefabs[UnityEngine.Random.Range(0, animalPrefabs.Count)];

            int x = targetNumberOfPairs - possiblePairs;
            if (x > 0 && animals.Count >= targetNumberOfAnimals - x)
            {
                //Debug.Log("Looking for a pair! " + possiblePairs + " - " + animals.Count);
                bool hit = false;
                foreach (Animal a in animals)
                {
                    if (a.animalName == animalType.name)
                    {
                        hit = true;
                        //Debug.Log("Found a " + animalType.name + " pair!");
                        break;
                    }
                }
                if (!hit) animalType = null;
            }
        } while (animalType == null);


        GameObject newAnimal = Instantiate(animalType,
                                           spawnPosition.position + new Vector3(UnityEngine.Random.Range(-spawnPosition.localScale.x / 2, spawnPosition.localScale.x / 2), 0, UnityEngine.Random.Range(-spawnPosition.localScale.z / 2, -spawnPosition.localScale.z / 2)),
                                           Quaternion.identity);
        Animal animalScript = newAnimal.GetComponent<Animal>();
        newAnimal.transform.localScale *= UnityEngine.Random.Range(1f - animalScript.scaleVariance, 1f + animalScript.scaleVariance);


        if (animals.Count(n => n.animalName == animalScript.animalName) == 1)
                possiblePairs++;

        animals.Add(animalScript);
        animalScript.SpawnAnimal(this);
        return animalScript;
    }

    public void SelectAnimal(Animal a)
    {
        if (!gameRunning || a == selectedAnimal || a != highlightedAnimal || !canSelect) return;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Spin);
        SwapLayer(a.gameObject, "Selected");
        a.outlineColour = 2;
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
            selectedAnimal.outlineColour = 0;
            selectedAnimal = a;
        }
        int randomIndex = UnityEngine.Random.Range(0, poofSounds.Count);
        poofAudioSource.PlayOneShot(poofSounds[randomIndex]);
    }

    public void MouseEnter(Animal a)
    {
        if (!gameRunning) return;
        if (highlightedAnimal != a) highlightedAnimal = a;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Clicked);
        if (selectedAnimal != a)
        {
            SwapLayer(a.gameObject, "Highlighted");
            a.outlineColour = 1;
        }
    }

    public void MouseExit(Animal a)
    {
        if (!gameRunning) return;
        if (highlightedAnimal == a) highlightedAnimal = null;
        a.GetComponent<AnimalAnimator>().ChangeAnimation(AnimalAnimator.Anim.Idle_A);
        if (selectedAnimal != a)
        {
            SwapLayer(a.gameObject, "Animal");
            a.outlineColour = 0;
        }
    }

    private bool PairAnimals(Animal a, Animal b)
    {
        if (a.animalName == b.animalName && a != b)
        {
            score += 2;
            pairAudioSource.Play();
            highlightedAnimal = null;
            selectedAnimal = null;
            animals.Remove(a);
            animals.Remove(b);
            possiblePairs--;
            if (animals.Count(n => n.animalName == a.animalName) > 1)
                possiblePairs++;
            StartCoroutine(MovePair(a, b));
            targetNumberOfAnimals++;
            if (targetNumberOfAnimals > 60) targetNumberOfAnimals = 60;
            targetNumberOfPairs = targetNumberOfAnimals / 4;
            StartCoroutine(SpawnAnimals(targetNumberOfAnimals - animals.Count));
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
        a.outlineColour = 0;

        yield return 0;

        b.gameObject.SetActive(true);
        b.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        puffParticles.transform.position = b.transform.position + new Vector3(0, 0.5f, 0);
        puffParticles.Play();
        b.paired = true;
        b.gameObject.layer = 0;
        SwapLayer(b.gameObject, "Default");
        b.outlineColour = 0;


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
