using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerObject : MonoBehaviour
{
    public void StartTheGame() {
        //SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
    }
}
