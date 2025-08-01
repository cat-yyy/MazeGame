using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TittleSceneManager : MonoBehaviour
{
    [Header("StartButtonで遷移させるシーン名")]
    [SerializeField] string sceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
