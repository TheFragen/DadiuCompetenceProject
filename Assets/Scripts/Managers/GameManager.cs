﻿using System.Collections;
using Gamelogic.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Gamelogic.Extensions.Algorithms;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public delegate void LevelGenerated();
    public static event LevelGenerated OnLevelGenerated;

    [SerializeField]
    private Items _ItemScriptableObject;
    [SerializeField]
    private List<GameObject> _players, _spawnedItems = new List<GameObject>();
    [SerializeField]
    private GameObject _activePlayer, _activePlayerCanvas;

    private Text _activePlayerText;
    private int _turnIndex = -1;
    private int _playerActions = 2;
    private int _activePlayerActions;
    private bool _turnHacks = false;
    private List<AbstractItem> _itemList;

    IEnumerator Start() {
        _itemList = new List<AbstractItem>(_ItemScriptableObject._itemList);
        yield return new WaitForSeconds(1);
        _activePlayerText = _activePlayerCanvas.GetComponent<Text>();
        SetupArtefacts();
        NextTurn(null);
        Level_PreGenerate.Instance.GenerateMaze();
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (_playerActions == 999)
            {
                _playerActions = 2;
            }
            else
            {
                _playerActions = 999;
            }
            
            print("Movement hacks  " + (_playerActions == 999 ? "activated" : "deactivated"));
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            _turnHacks = !_turnHacks;
            print("Turn hacks " + (_turnHacks ? "activated" : "deactivated"));
        }
    }

    public void SetLevelGenerated()
    {
        if (OnLevelGenerated != null)
        {
            OnLevelGenerated.Invoke();
        }
    }

    void SetupArtefacts()
    {
        Debug.Assert(_ItemScriptableObject != null);
        List<AbstractItem> items = _ItemScriptableObject._itemList.Where(o => o.isArtefact).ToList();
        foreach (var player in _players)
        {
            AbstractItem randomArtefact = items[Random.Range(0, items.Count)];
            player.GetComponent<Inventory>().SetArtefact(randomArtefact);
            items.Remove(randomArtefact);
            if (player.tag == "Player" || player.tag == "AI")
            {
                JuiceController.Instance.SetNeededArtefact(randomArtefact);
            }
        }
    }

    public List<AbstractItem> GetItemList()
    {
        return _itemList;
    }

    public void AddSpawnedItem(GameObject item, AbstractItem aItem)
    {
        _spawnedItems.Add(item);
        if (aItem.isArtefact)
        {
            _itemList.Remove(aItem);
        }
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
        if (_turnHacks)
        {
            return true;
        }
        return _activePlayer == player;
    }

    public void CreatePlayer(GameObject player)
    {
        _players.Add(player);
    }

    public bool PerformAction(GameObject player)
    {
        if (_turnHacks)
        {
            return true;
        }
        if (player == _activePlayer)
        {
            if (_activePlayerActions > 0)
            {
                _activePlayerActions--;
                JuiceController.instance.SetActionsLeft(_activePlayerActions);
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
            JuiceController.instance.SetActionsLeft(_activePlayerActions);
            return true;
        }
        return false;
    }

    public void RemoveAllItems(GameObject player)
    {
        Inventory inventoryComponent = player.GetComponent<Inventory>();
        List<AbstractItem> inventory = new List<AbstractItem>(inventoryComponent.GetInventory());
        foreach (var item in inventory)
        {
            inventoryComponent.RemoveItem(item);
        }
        inventoryComponent.ArtefactFound = false;
    }

    public void NextTurn(GameObject player)
    {
        if (player != _activePlayer)
        {
            return;
        }
        _activePlayer = null;
        _turnIndex++;
        if (_turnIndex == _players.Count)
        {
            _turnIndex = 0;
        }
        Camera.main.GetComponent<FollowPlayer>().SetPlayer(_players[_turnIndex]);
        StartCoroutine(DelayTurn());
    }

    private IEnumerator DelayTurn()
    {
        yield return new WaitForSeconds(1.5f);
        _activePlayer = _players[_turnIndex];
        _activePlayerActions = _playerActions;
        JuiceController.instance.SetEndTurnButton(_activePlayer);
        JuiceController.instance.SetActionsLeft(_activePlayerActions);
        _activePlayerText.text = _activePlayer.name + " is playing his turn.";
    }
}