using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDigging : MonoBehaviour
{

    private SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D trigger)
    {
        sr.color = new Color32(68, 68, 68, 255);
    }


}
