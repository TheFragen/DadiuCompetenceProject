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
    private int _playerActions = 2;
    private int _activePlayerActions;

    IEnumerator Start() {
        yield return new WaitForSeconds(1);
        _activePlayerText = _activePlayerCanvas.GetComponent<Text>();
        NextTurn(null);
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            _playerActions = 999;
            print("Turn hacks activated");
        }
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

    public bool PerformAction(GameObject player)
    {
        if (player == _activePlayer)
        {
            if (_activePlayerActions > 0)
            {
                _activePlayerActions--;
                return true;
            }
            return false;
        }
        return false;
    }

    public bool IncreaseAction(GameObject player, int amount)
    {
        if (player == _activePlayer)
        {
            _activePlayerActions += amount;
            return true;
        }
        return false;
    }

    public void NextTurn(GameObject player)
    {
        if (player != _activePlayer) return;
        _activePlayer = null;
        Camera.main.GetComponent<FollowPlayer>().SetPlayer(_players[_turnIndex + 1]);
        StartCoroutine(DelayTurn());
    }

    private IEnumerator DelayTurn()
    {
        yield return new WaitForSeconds(1.5f);
        _activePlayer = _players[++_turnIndex];
        _activePlayerActions = _playerActions;

        if (_turnIndex == _players.Count - 1) {
            _turnIndex = -1;
        }
        _activePlayerText.text = _activePlayer.name + " is playing his turn.";
    }
}