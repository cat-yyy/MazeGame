using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var current = Keyboard.current;

        if (current == null)
        {
            return;
        }

        var aKey = current.aKey;

        if (aKey.wasPressedThisFrame)
        {
            Debug.Log("A�L�[�������ꂽ");
        }

        if (aKey.wasReleasedThisFrame)
        {
            Debug.Log("A�L�[�������ꂽ");
        }

        if (aKey.isPressed)
        {
            Debug.Log("A�L�[��������Ă���!");
        }
    }
}
