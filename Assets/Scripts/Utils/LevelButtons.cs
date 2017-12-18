using UnityEngine;
using EasyButtons;
public class LevelButtons : MonoBehaviour
{
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
    void ClearMap() {
        Level_PreGenerate.Instance.ClearMap();
        Grid.Instance.ClearGrid();
    }
}