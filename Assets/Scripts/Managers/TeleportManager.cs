using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeleportManager : Singleton<TeleportManager>
{
    [SerializeField]
    private Grid _Grid;
    [SerializeField]
    private int _MaxTeleportDistance;
    [SerializeField]
    private GameObject _PlayerInUse;
    [SerializeField]
    private GameObject _SelectionCube;

    private bool _LevelIsCreated;
    private Renderer _SelectionRenderer;
    private float _clickTime;

    void OnEnable()
    {
        GameManager.OnLevelGenerated += SetLevelCreated;
    }

    void OnDisable()
    {
        GameManager.OnLevelGenerated -= SetLevelCreated;
    }
    // Use this for initialization
    void Start()
    {
        _SelectionCube.SetActive(false);
        _SelectionRenderer = _SelectionCube.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_LevelIsCreated || _PlayerInUse == null) return;
        _SelectionCube.SetActive(true);
        FindNodePos();
    }

    void FindNodePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPos = hit.point;
            worldPos.y = 0;
              Node closestNode =
                  _Grid.WorldPosToNode(worldPos);
            _SelectionCube.transform.position = closestNode._WorldPos + new Vector3(0,.5f,0);

            if (ManhattenDistance(_PlayerInUse.transform.position,
                    closestNode._WorldPos) > _MaxTeleportDistance)
            {
                _SelectionRenderer.material.color = Color.red;
            }
            else
            {
                _SelectionRenderer.material.color = Color.green;
            }

            if (Input.GetMouseButtonUp(0) && Time.time > _clickTime + .5f)
            {
                _PlayerInUse.transform.position =
                    _SelectionCube.transform.position;
                _PlayerInUse = null;
                _SelectionCube.SetActive(false);
            }
        }
    }

    int ManhattenDistance(Vector3 s, Vector3 e)
    {
        float dx = Mathf.Abs(s.x - e.x);
        float dz = Mathf.Abs(s.z - e.z);
        int dist = Mathf.RoundToInt(dx + dz);
        return dist;
    }

    void SetLevelCreated() {
        _LevelIsCreated = true;
    }


    public void SetPlayerInUse(GameObject player)
    {
        if (_PlayerInUse == null)
        {
            _clickTime = Time.time;
            _PlayerInUse = player;
        }
    }

    public void AITeleport(GameObject AI)
    {
        //TODO: Implement teleport for AI
        print("Missing AI teleport implementation");
    }
}
