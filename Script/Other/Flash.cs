using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public MeshRenderer target;
    private MeshRenderer thisMesh;
    void Start()
    {
        thisMesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        thisMesh.material.color = target.material.color;
    }
}
