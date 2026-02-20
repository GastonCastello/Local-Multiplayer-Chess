using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    MeshRenderer meshR;

    MeshRenderer meshRenderer 
    {
        get
        {
            if (meshR == null)
                meshR = GetComponent<MeshRenderer>();
            return meshR;
        }
    }

    public void SetSingleMaterial(Material material)
    {
        meshRenderer.material = material;
    }
}
