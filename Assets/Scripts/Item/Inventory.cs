using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public bool ArtefactFound = false;
    [SerializeField]
    private GameObject _canvasPanel, _buttonPrefab;
    [SerializeField]
    private AbstractItem _ArtefactToFind;
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
        item.owner = gameObject;
        _Inventory.Add(item);
        GameManager.Instance.RemoveSpawnedItem(item.gameObject);

        if (_ArtefactToFind.itemName.Equals(item.itemName))
        {
            JuiceController.Instance.AnnounceArtefact(item, null);
            ArtefactFound = true;
        }

        if (_canvasPanel != null)
        {
            CreateCanvasButton(item);
        }
    }

    public void SetArtefact(AbstractItem item)
    {
        _ArtefactToFind = item;
    }

    public void RemoveItem(AbstractItem item)
    {
        // Check if the item has already been removed - such as in a foreach loop
        if (!_Inventory.Contains(item))
        {
            return;
        }
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

    public bool HasAnyArtifact()
    {
        foreach (var item in _Inventory)
        {
            if (item.isArtefact)
            {
                return true;
            }
        }
        return false;
    }

    void CreateCanvasButton(AbstractItem item)
    {
        GameObject tmpButton = Instantiate(_buttonPrefab);
        tmpButton.GetComponent<Button>().onClick.AddListener(item.Use);
        tmpButton.GetComponent<Image>().sprite = item.sprite;
        tmpButton.name = item.itemName;
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