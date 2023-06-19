using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalPool : MonoBehaviour
{
    [SerializeField] 
    private int _animalCount = 10;
    [SerializeField]
    private Animal _sheepPrefab;
    [SerializeField]
    private Animal _rabbitPrefab;
    [SerializeField]
    private Animal _dogPrefab;
    private Dictionary<AnimalType, List<Animal>> _animalPools = new Dictionary<AnimalType, List<Animal>>();

    private void Awake()
    {
        _animalPools.Add(AnimalType.Dog,new List<Animal>());
        _animalPools.Add(AnimalType.Sheep,new List<Animal>());
        _animalPools.Add(AnimalType.Rabbit,new List<Animal>());
        for(int i=0; i<_animalCount; i++)
        {

            _animalPools[AnimalType.Dog].Add(CreateAnimal(_dogPrefab));
            _animalPools[AnimalType.Sheep].Add(CreateAnimal(_sheepPrefab));
            _animalPools[AnimalType.Rabbit].Add(CreateAnimal(_rabbitPrefab));
        }
    }

    Animal CreateAnimal(Animal prefab)
    {
        Animal animal = Instantiate(prefab);
        animal.gameObject.SetActive(false);
        return animal;
    }

    public bool TryGetAnimal(AnimalType type, out Animal animal)
    {

        List<Animal> pool = _animalPools[type];
        Debug.Log("TryGetAnimal " + pool.Count);
        if (pool.Count > 0)
        {
            Debug.Log("GetAnimal");
            animal = pool.First();
            pool.Remove(animal);
            animal.gameObject.SetActive(true);
            return true;
        }

        animal = null;
        return false;
    }

    public void ReturnAnimal(Animal animal)
    {
        animal.gameObject.SetActive(false);
        _animalPools[animal.AnimalType].Add(animal);
    }
}
