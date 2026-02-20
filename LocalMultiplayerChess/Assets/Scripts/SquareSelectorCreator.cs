using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    [SerializeField] Material freeSquareMat;
    [SerializeField] Material enemySquareMat;
    [SerializeField] GameObject selectorPrefab;
    List<GameObject> instantiatedSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3,bool> squareData)
    {
        ClearSelection();

        foreach (var item in squareData)
        {
            GameObject selector = Instantiate(selectorPrefab, new Vector3(item.Key.x-0.01f,item.Key.y-0.045f,item.Key.z+0.004f), Quaternion.identity); // un poco desfasado arreglado con valores hardcodeados
            instantiatedSelectors.Add(selector);
            foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
            {
                setter.SetSingleMaterial(item.Value ? freeSquareMat : enemySquareMat);
            }
        }
    }

    public void ClearSelection()
    {
        foreach (var item in instantiatedSelectors)
        {
            Destroy(item.gameObject);
        }
    }
}
