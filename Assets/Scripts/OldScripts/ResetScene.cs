using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public static void Reset()
    {
        SceneManager.LoadScene(0);
    }

}
