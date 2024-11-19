using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    void Start()
    {
        Debug.Log(textMeshProUGUI.GetPreferredValues());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
