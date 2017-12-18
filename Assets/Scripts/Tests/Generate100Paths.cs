using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Generate100Paths : MonoBehaviour
{
    public PathFinding _Path;
    public Grid _Grid;

    IEnumerator TestCo()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
     //   List<List<Node>> paths = new List<List<Node>>();
        Dictionary<Vector3, Node> grid = _Grid.GetGrid();

      //  for (int c = 500; c <= 500; c += 500) {
        //    float t = 0;
       //     Level_PreGenerate.Instance.SetNumTilesToSpawn(c);
        //    Level_PreGenerate.Instance.GenerateMaze();
        //    yield return new WaitForSeconds(1);
         //   grid = _Grid.GetGrid();
            for (int v = 0; v < 3; v++) {
              //  sw.Stop();
              //  sw.Reset();
              //  sw.Start();
                for (int i = 0; i < 100; i++) {
                    Vector3 randomPos = grid.ElementAt(Random.Range(0, grid.Count)).Value._WorldPos;
                    List<Node> path = _Path.GeneratePath(Vector3.zero, randomPos);
                  //  paths.Add(path);
                }
              //  t += sw.ElapsedMilliseconds;
           //     Debug.Log(c + " run: " + v);
             //   yield return null;
            }
          /*  t = t / 3;
            Debug.Log(c + " - average time for 3 runs: " + t);
            Clear();*/
       //     yield return null;
     //   }

        /*
        for (int i = 0; i < 100; i++) {
            Vector3 randomPos = grid.ElementAt(Random.Range(0, grid.Count)).Value._WorldPos;
            List<Node> path = _Path.GeneratePath(Vector3.zero, randomPos);
            paths.Add(path);
        }

        bool testResult = true;
        foreach (var p in paths) {
            testResult = p != null;
        }*/
        Debug.Log("Time: " + sw.ElapsedMilliseconds);
        Debug.Log(GetType().Name + " finished");
        yield return null;
    }

    public void Test()
    {
        StartCoroutine(TestCo());
        //   Debug.Log("Time: " +sw.Elapsed.TotalSeconds);
    }

    private void Clear()
    {
        Level_PreGenerate.Instance.ClearMap();
        Grid.Instance.ClearGrid();
    }
}
