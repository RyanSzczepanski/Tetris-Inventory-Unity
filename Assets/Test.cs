using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
