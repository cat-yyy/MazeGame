using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionExample : MonoBehaviour
{
    [SerializeField] private InputAction _action;

    private void OnEnable()
    {
        _action?.Enable();
    }

    private void OnDisable()
    {
        _action?.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_action == null)
        {
            return;
        }

        var value = _action.ReadValue<float>();

        Debug.Log($"Action‚Ì“ü—Í’l:{value}");
        
    }
}
