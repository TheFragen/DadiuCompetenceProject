using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject _canvasPanel, _buttonPrefab;
    [SerializeField]
    private List<AbstractItem> _Inventory;

    private Dictionary<AbstractItem, GameObject> itemButtonDictionary;

    private HumanMotor _hm;

    void Start()
    {
        itemButtonDictionary = new Dictionary<AbstractItem, GameObject>();
    }

    void OnEnable()
    {
        _hm = GetComponent<HumanMotor>();
        _hm.OnItemPickup += PickupItem;
    }

    void OnDisable()
    {
        _hm.OnItemPickup -= PickupItem;
    }

    void PickupItem(AbstractItem item)
    {
        print("Received event.");
        item.owner = gameObject;
        _Inventory.Add(item);
        GameManager.Instance.RemoveSpawnedItem(item.gameObject);

        if (_canvasPanel != null)
        {
            CreateCanvasButton(item);
        }
    }

    public void RemoveItem(AbstractItem item)
    {
        _Inventory.Remove(item);
        if (_canvasPanel != null)
        {
            RemoveCanvasButton(item);
        }
        _Inventory.TrimExcess();
    }

    public List<AbstractItem> GetInventory()
    {
        return _Inventory;
    }

    void CreateCanvasButton(AbstractItem item)
    {
        GameObject tmpButton = Instantiate(_buttonPrefab);
        tmpButton.GetComponent<Button>().onClick.AddListener(item.Use);
        tmpButton.GetComponent<Image>().sprite =
            item.gameObject.GetComponent<SpriteRenderer>().sprite;
        tmpButton.name = item.name;
        tmpButton.transform.SetParent(_canvasPanel.transform);
        itemButtonDictionary.Add(item, tmpButton);
    }

    void RemoveCanvasButton(AbstractItem item)
    {
        GameObject tmp = itemButtonDictionary[item];
        Destroy(tmp);
        itemButtonDictionary.Remove(item);
    }
}