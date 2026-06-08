using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewStall : MonoBehaviour
{
    public bool IsColliding { get; private set; }  

    public MeshRenderer[] meshRenderers;

    [SerializeField] Material redMaterial;
    [SerializeField] Material woodenMaterial;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Debug 01");
        IsColliding = true;
        SetMaterial(redMaterial);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Debug 01");
        IsColliding = true;
        SetMaterial(redMaterial);
    }

    private void OnTriggerExit(Collider other)
    {
        IsColliding = false;
        SetMaterial(woodenMaterial);
    }

    private void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = material;
        }
    }
}
