using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField]
    private GameObject Sky;
    [SerializeField]
    private GameObject Decorate;
    [SerializeField]
    private GameObject Ground;
    [SerializeField]
    private GameObject Dark;
    protected internal void removeAll()
    {
        Destroy(Sky);
        Destroy(Decorate);
        Destroy(Ground);
        Destroy (Dark);
    }
}
