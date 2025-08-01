using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject DifficultyLevelPanel;
    

    void Start()
    {
        DifficultyLevelPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    ///スタートボタンが押されたときに呼ぶメソッド
    /// </summary>
    public void PushStart()
    {
        MenuPanel.SetActive(false);
        DifficultyLevelPanel.SetActive(true);
    }

}
