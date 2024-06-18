using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }
 
    public void Open()
    {
        gameObject.SetActive(true);  
    }
    public void Close()
    {
      gameObject.SetActive(false);  
    }
}


