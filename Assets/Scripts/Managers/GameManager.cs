using System.Collections;
using Gamelogic.Extensions;
using UnityEngine;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private List<GameObject> _players = new List<GameObject>();

    [SerializeField]
    private List<GameObject> _spawnedItems = new List<GameObject>();

    [SerializeField]
    private GameObject _activePlayer, _activePlayerCanvas;

    private Text _activePlayerText;
    private int _turnIndex = -1;

    IEnumerator Start() {
        yield return new WaitForSeconds(1);
        _activePlayerText = _activePlayerCanvas.GetComponent<Text>();
        NextTurn();
    }

    public void AddSpawnedItem(GameObject item)
    {
        _spawnedItems.Add(item);
    }

    public void RemoveSpawnedItem(GameObject item)
    {
        _spawnedItems.Remove(item);
    }

    public List<GameObject> GetSpawnedItems()
    {
        return _spawnedItems;
    }

    public bool IsPlayerTurn(GameObject player)
    {
        return _activePlayer == player;
    }

    public void CreatePlayer(GameObject player)
    {
        _players.Add(player);
    }

    public void NextTurn()
    {
        _activePlayer = _players[++_turnIndex];
        if (_turnIndex == _players.Count-1)
        {
            _turnIndex = -1;
        }
        _activePlayerText.text = _activePlayer.name +" is playing his turn.";
    }


}