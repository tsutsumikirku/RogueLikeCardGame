using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BackgroundMove : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    private void FixedUpdate()
    {
        float temp = (cam.transform.position.x);

        transform.position = new Vector3(startpos, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}