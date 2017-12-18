using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestGenerateLevels : MonoBehaviour
{
    [SerializeField]
    private Level_PreGenerate generator;

    private IEnumerator TestWithYield()
    {
        Stopwatch sw = new Stopwatch();
        for (int size = 500; size <= 10000; size += 500) {
            float time = 0;
            for (int iteration = 0; iteration < 3; iteration++) {
                sw.Stop();
                sw.Reset();
                sw.Start();
                Clear();
                yield return new WaitForSeconds(1);
                generator.SetNumTilesToSpawn(size);
                yield return new WaitForSeconds(1);
                generator.GenerateMaze();
                time += sw.ElapsedMilliseconds - 2000;
                UnityEngine.Debug.Log("Size: " + size +" iteration " + (iteration+1) +"/3");
                yield return new WaitForEndOfFrame();
            }
            time = time / 3;
            UnityEngine.Debug.Log("Size: " + size + " - average after 3 iterations: " + time);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Test()
    {
        StartCoroutine(TestWithYield());
    }

    private void Clear() {
        generator.ClearMap();
        Grid.Instance.ClearGrid();
    }

}
