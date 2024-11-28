using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    public VideoPlayer vp;
    [SerializeField]
    int differential;

    void Start()
    {
        double lengthVid;
        lengthVid = vp.clip.length / vp.playbackSpeed;
        Invoke("LoadNextScene", (float)lengthVid - differential);
    }

    public void LoadNextScene()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(nextScene);
    }
}
