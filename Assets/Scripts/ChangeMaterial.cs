using System;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] GameObject point;
    public void ChangeCurrentMaterial(Material activationMarkMaterial)
    {
        GetComponent<Renderer>().material = activationMarkMaterial;
        point.GetComponent<Renderer>().material = activationMarkMaterial;
    }
}
