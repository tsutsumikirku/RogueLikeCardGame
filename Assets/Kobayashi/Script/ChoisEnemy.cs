using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoisEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseClickGetEnemy();
        }
    }
    CharacterBase MouseClickGetEnemy()
    {
#nullable enable
        CharacterBase? hitGameObject;
#nullable disable
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit=Physics2D.Raycast(ray.origin,ray.direction,100);
        Debug.DrawRay(ray.origin,ray.direction*100,Color.red);
        if (hit.transform != null)
        {
            hitGameObject = hit.transform.gameObject.GetComponent<CharacterBase>();
            if (hitGameObject != null)
            {
                Debug.Log(hitGameObject);
                return hitGameObject;
            }
        }
        return null;
    }
}
