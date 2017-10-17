using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject _canvasPanel, _buttonPrefab;
    [SerializeField]
    private List<Item> _Inventory;

    private HumanMotor _hm;

    void OnEnable()
    {
        _hm = GetComponent<HumanMotor>();
        _hm.OnItemPickup += PickupItem;
    }
    void OnDisable()
    {
        _hm.OnItemPickup -= PickupItem;
    }

    void PickupItem(Item item)
    {
        print("Received event.");
        _Inventory.Add(item);
        GameManager.Instance.RemoveSpawnedItem(item.gameObject);

        if (_canvasPanel != null)
        {
            CreateCanvasButton(item);
        }
    }

    void CreateCanvasButton(Item item)
    {
        GameObject tmpButton = Instantiate(_buttonPrefab);
        tmpButton.GetComponent<Button>().onClick.AddListener(item.Use);
        tmpButton.GetComponent<Image>().sprite = item.gameObject.GetComponent<SpriteRenderer>().sprite;
        tmpButton.name = item.name;
        tmpButton.transform.SetParent(_canvasPanel.transform);
    }
}