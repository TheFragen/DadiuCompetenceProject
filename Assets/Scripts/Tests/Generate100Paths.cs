using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generate100Paths : MonoBehaviour
{
    public PathFinding _Path;
    public Grid _Grid;

	// Use this for initialization
	void Start () {
		
	}

    public void Test()
    {
        List<List<Node>> paths = new List<List<Node>>();
        Dictionary<Vector3, Node> grid = _Grid.GetGrid();

        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPos = grid.ElementAt(Random.Range(0, grid.Count)).Value._WorldPos;
            List<Node> path = _Path.GeneratePath(Vector3.zero, randomPos);
            paths.Add(path);
        }

        foreach (var p in paths)
        {
            Debug.Assert(p != null);
        }
        Debug.Log(GetType().Name + " finished");
    }
}
