using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using EasyButtons;
using UnityEditor.VersionControl;

public class LevelButtons : MonoBehaviour {
    [SerializeField]
    Tile spawningObject;
    private List<GameObject> types;
    [SerializeField]
    private List<Tile> tileTypes;

    [SerializeField]
    private int tilesToSpawn = 10;

    private Vector3 spawnPos;

    Dictionary<Vector3, Tile> dict = new Dictionary<Vector3, Tile>();

    [Button]
    void GenerateMaze()
    {
        Level_PreGenerate.Instance.GenerateMaze();
    }

    [Button]
    void GenerateWithDelay()
    {
        Level_PreGenerate.Instance.GenerateWithDelay();
    }

    [Button]
    void RevealMap()
    {
        Level_PreGenerate.Instance.RevealMap();
    }

    [Button]
    void GenerateMazeInSeperateThread()
    {
        StartCoroutine(ThreadStarter());
    }

    IEnumerator ThreadStarter()
    {
        spawnPos = spawningObject.transform.position;
        types = Level_PreGenerate.Instance.GetTileTypes();
        tileTypes = new List<Tile>();
        foreach (var t in types) {
            tileTypes.Add(t.GetComponent<Tile>());
        }

        
        Thread thread = new Thread(Test);
        thread.Start();
        while (thread.IsAlive)
        {
            yield return null;
        }
        print("DICT main: " + dict.Count);

        for(int i = 0; i < dict.Count; i++)
        {
            if (i == dict.Count - 1)
            {
                Instantiate(dict.ElementAt(i).Value.prefab, dict.ElementAt(i).Key, Quaternion.Euler(0,0,90));
            }
            if (dict.ElementAt(i).Value.prefab == null)
            {
                print(dict.ElementAt(i).Value.gameObject.name);
            }
            else
            {
                Instantiate(dict.ElementAt(i).Value.prefab, dict.ElementAt(i).Key, Quaternion.identity);
            }
        }
    }

    void Test()
    {
        print("Thread start");
        Level_Threading lt = new Level_Threading();

        Dictionary<Vector3, Tile> dDict = lt.GenerateMazeInThread(spawningObject, spawnPos, tilesToSpawn, tileTypes);
      //  UnityMainThreadDispatcher.Instance().Enqueue(() => { dict = dDict; });
        print("DICT: " + dDict.Count);

        foreach (var node in dDict)
        {
            print(node.Key);
        }
        dict = dDict;

        print("Thread end");
    }
}