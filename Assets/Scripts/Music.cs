using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public string disableScene; 
    public static Music instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == disableScene)
            gameObject.SetActive(false);
    }
}
