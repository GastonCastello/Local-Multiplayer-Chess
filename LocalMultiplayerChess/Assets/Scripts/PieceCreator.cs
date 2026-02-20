using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCreator : MonoBehaviour
{
    [SerializeField] GameObject[] piecesPrefabs;
    [SerializeField] Material blackMaterial;
    [SerializeField] Material whiteMaterial;

    Dictionary<string, GameObject> nameToPiece = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (var item in piecesPrefabs)
        {
            nameToPiece.Add(item.GetComponent<Piece>().GetType().ToString(), item);
        }
    }

    public GameObject CreatePiece(Type type)
    {
        GameObject prefab = nameToPiece[type.ToString()];

        if (prefab)
        {
            GameObject newPiece = Instantiate(prefab);

            Debug.Log("piece creatoin of " + newPiece);

            return newPiece;
        }
        return null;
    }

    public Material TeamMaterial(TeamColor team)
    {
        return team == TeamColor.White ? whiteMaterial : blackMaterial;
    }
}
