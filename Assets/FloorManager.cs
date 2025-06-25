using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [System.Serializable]
    public struct DoorConfiguration
    {
        public int plantIndex;   // Index in the plants array, -1 if none
        public int doormatIndex; // Index in the doormats array, -1 if none
        public int objectIndex;  // Index in the objects array, -1 if none
    }

    [System.Serializable]
    public struct DoorVisuals
    {
        public Transform plantTransform;
        public Transform doormatTransform;
        public Transform objectTransform;

        [HideInInspector] public GameObject activePlant;
        [HideInInspector] public GameObject activeDoormat;
        [HideInInspector] public GameObject activeObject;
    }

    [Header("Configuration")]
    [SerializeField, Range(0f, 1f)] private float chanceOfSpawningObject = 0.25f;
    [SerializeField, Range(0f, 1f)] private float chanceOfSpawningMat = 0.25f;
    [SerializeField, Range(0f, 1f)] private float chanceOfSpawningPlant = 0.25f;
    private int numberOfFloors = 5;
    public GameObject[] plants;
    public GameObject[] doormats;
    public GameObject[] objects;
    public GameObject[] paintings;

    [Header("Scene References")]
    public DoorVisuals[] doorVisuals; // 14 door visuals with specific transform locations
    public Transform paintingPosition; // Shared painting position

    private List<GameObject> shuffledPaintings;
    private int currentFloorIndex = 0;

    private DoorConfiguration[][] savedDoorConfigurations;
    private int[] savedPaintingIndices;
    private GameObject activePainting;


    public void Initalize(int numberOfFloors)

    {
        this.numberOfFloors = numberOfFloors;
        InitializePaintings();
        GenerateFloorConfigurations();
        InstantiateAllDecor();
        ApplyFloor(0);
    }

    void InitializePaintings()
    {
        shuffledPaintings = new List<GameObject>(paintings);
        for (int i = 0; i < shuffledPaintings.Count; i++)
        {
            GameObject temp = shuffledPaintings[i];
            int randomIndex = Random.Range(i, shuffledPaintings.Count);
            shuffledPaintings[i] = shuffledPaintings[randomIndex];
            shuffledPaintings[randomIndex] = temp;
        }
    }

    void GenerateFloorConfigurations()
    {
        savedDoorConfigurations = new DoorConfiguration[numberOfFloors][];
        savedPaintingIndices = new int[numberOfFloors];

        for (int floor = 0; floor < numberOfFloors; floor++)
        {
            savedDoorConfigurations[floor] = new DoorConfiguration[doorVisuals.Length];
            for (int door = 0; door < doorVisuals.Length; door++)
            {
                savedDoorConfigurations[floor][door] = new DoorConfiguration
                {
                    plantIndex = Random.value <= chanceOfSpawningPlant && plants.Length > 0 ? Random.Range(0, plants.Length) : -1,
                    doormatIndex = Random.value <= chanceOfSpawningMat && doormats.Length > 0 ? Random.Range(0, doormats.Length) : -1,
                    objectIndex = Random.value <= chanceOfSpawningObject && objects.Length > 0 ? Random.Range(0, objects.Length) : -1
                };
            }

            savedPaintingIndices[floor] = floor < shuffledPaintings.Count ? floor : -1;
        }
    }

    void InstantiateAllDecor()
    {
        for (int i = 0; i < doorVisuals.Length; i++)
        {
            doorVisuals[i].activePlant = InstantiateAllFromArray(plants, doorVisuals[i].plantTransform);
            doorVisuals[i].activeDoormat = InstantiateAllFromArray(doormats, doorVisuals[i].doormatTransform);
            doorVisuals[i].activeObject = InstantiateAllFromArray(objects, doorVisuals[i].objectTransform);
        }

        if (paintingPosition.childCount > 0)
        {
            foreach (Transform child in paintingPosition)
                Destroy(child.gameObject);
        }

        if (shuffledPaintings.Count > 0)
        {
            activePainting = Instantiate(shuffledPaintings[0], paintingPosition);
        }
    }

    GameObject InstantiateAllFromArray(GameObject[] array, Transform parent)
    {
        GameObject container = new GameObject("DecorContainer");
        container.transform.SetParent(parent);
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;

        foreach (GameObject prefab in array)
        {
            GameObject obj = Instantiate(prefab, container.transform);
            obj.SetActive(false);
        }
        return container;
    }

    public void ApplyFloor(int floorIndex)
    {
        if (floorIndex < 0 || floorIndex >= savedDoorConfigurations.Length) return;

        currentFloorIndex = floorIndex;

        for (int i = 0; i < doorVisuals.Length; i++)
        {
            DoorConfiguration config = savedDoorConfigurations[floorIndex][i];

            SetActiveChild(doorVisuals[i].activePlant, config.plantIndex);
            SetActiveChild(doorVisuals[i].activeDoormat, config.doormatIndex);
            SetActiveChild(doorVisuals[i].activeObject, config.objectIndex);
        }

        SetActiveChild(paintingPosition.gameObject, savedPaintingIndices[floorIndex]);
    }

    void SetActiveChild(GameObject parent, int index)
    {
        if (parent == null) return;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }
}
