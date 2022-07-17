using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    enum State
    {
        Spawning,
        Fighting
    }
    State state;

    [Header("Object Prefabs")]
    [SerializeField] GameObject bat;
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject spider;
    [SerializeField] GameObject ogre;
    [SerializeField] GameObject warning;
    [SerializeField] GameObject bigWarning;
    private GameObject[] enemyPrefabs;

    [Header("Spawn Scheduler")]
    [SerializeField] Vector4[] scheduler;
    [SerializeField] float warningDuration;
    // (position.x, position.y, Enemy ID, Wave)
    private List<List<Vector3>> enemySchedule = new List<List<Vector3>>();

    private int wave = 0;
    [SerializeField] float maxTime = 30;
    [SerializeField] float timeMultiplier = 0.9f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        List<Vector3> waveI = new List<Vector3>();
        foreach (Vector4 v in scheduler)
        {
            if (v.w > i)
            {
                enemySchedule.Add(waveI);
                waveI = new List<Vector3>();
                i++;
            }
            waveI.Add(new Vector3(v.x, v.y, v.z));
        }
        enemySchedule.Add(waveI);

        enemyPrefabs = new GameObject[]
        {
            bat, skeleton, spider, ogre
        };

        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Spawning:
                break;
            case State.Fighting:
                timer -= Time.deltaTime;
                if (GameObject.FindWithTag("Enemy") == null || timer <= 0)
                {
                    wave++;
                    if (wave >= enemySchedule.Count)
                    {
                        wave = 0;
                    }
                    StartCoroutine(SpawnEnemies());
                    state = State.Spawning;
                    maxTime *= timeMultiplier;
                }
                break;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        // Set Warnings
        List<GameObject> warnings = new List<GameObject>();
        foreach (Vector3 v in enemySchedule[wave])
        {
            GameObject warn = Instantiate(v.z == 3 ? bigWarning : warning);
            warn.transform.position = new Vector2(v.x, v.y);
            warn.transform.localScale = Vector2.zero;
            LeanTween.scale(warn, Vector2.one, 0.25f).setEaseOutBack();
            warnings.Add(warn);
        }
        yield return new WaitForSeconds(warningDuration);

        // Remove Warnings
        foreach (GameObject warn in warnings)
        {
            LeanTween.scale(warn, Vector2.one * 3f, 0.25f).setEaseInCubic();
            LeanTween.alpha(warn, 0, 0.25f).setEaseOutCubic().setDestroyOnComplete(true);
        }

        yield return new WaitForSeconds(0.25f);


        foreach (Vector3 v in enemySchedule[wave])
        {
            GameObject enemy = Instantiate(enemyPrefabs[(int)v.z]);
            enemy.transform.position = new Vector2(v.x, v.y);
        }

        timer = maxTime;
        state = State.Fighting;
    }
}
