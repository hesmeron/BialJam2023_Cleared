using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Mast : MonoBehaviour
{
    [SerializeField] 
    private float _height = 1f;
    [SerializeField] 
    private float _flagHeight = 0.15f;
    [SerializeField]
    private GameObject[] _flagPrefabs;


    private List<GameObject>[] _flags;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up*_height));
    }

    public void VisualizeNeededAnimals(int[] neededAnimals)
    {

        _flags = new List<GameObject>[3];
        for (var index = 0; index < _flags.Length; index++)
        {
            _flags[index] = new List<GameObject>();
        }

        int count = 0;
        for (var i = 0; i < neededAnimals.Length; i++)
        {
            int neededAnimal = neededAnimals[i];
            for (int j = 0; j < neededAnimal; j++)
            {
                GameObject flag = Instantiate(_flagPrefabs[i], transform);
                float y = (_height - (_flagHeight * count));
                Vector3 localPosition = flag.transform.localPosition;
                localPosition = new Vector3(localPosition.x, y, localPosition.z);
                flag.transform.localPosition = localPosition;
                _flags[i].Add(flag);
                count++;
            }
        }
    }

    public void RemoveFlag(int index)
    {
        GameObject flag = _flags[index].First();
        _flags[index].Remove(flag);

        flag.transform.DOMove(transform.position, 1.2f);
        flag.transform.DOScale(Vector3.zero, 1.5f).onComplete += () =>
        {
            Destroy(flag.gameObject);
        };
        
    }
}
