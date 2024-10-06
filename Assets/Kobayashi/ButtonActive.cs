using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActive : MonoBehaviour
{
    [SerializeField]List<GameObject> gameObjects = new List<GameObject>();
    public void HideButton(bool hideMode)
    {
        foreach (GameObject obj in gameObjects)
        {
            obj.SetActive(hideMode);
        }
    }
}
