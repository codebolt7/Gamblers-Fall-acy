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
            GameObject warn = Instantiate(warning);
            warn.transform.position = new Vector2(v.x, v.y);
            warnings.Add(warn);
        }
        yield return new WaitForSeconds(warningDuration);

        // Remove Warnings
        foreach (GameObject warn in warnings)
        {
            Destroy(warn);
        }

        foreach (Vector3 v in enemySchedule[wave])
        {
            GameObject enemy = Instantiate(enemyPrefabs[(int)v.z]);
            enemy.transform.position = new Vector2(v.x, v.y);
        }

        timer = maxTime;
        state = State.Fighting;
    }
}