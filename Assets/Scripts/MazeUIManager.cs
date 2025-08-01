using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class MazeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private LANGenerator LANGenerator;
    void Start()
    {
        buttonPanel.SetActive(false);
    }

    void Update()
    {
        
    }


    /// <summary>
    ///ButtonPanelのオンオフ切り替えメソッド 
    /// </summary>
    public void ExecuteButtonPanel(bool active)
    {
        buttonPanel.SetActive(active);
    }
}
