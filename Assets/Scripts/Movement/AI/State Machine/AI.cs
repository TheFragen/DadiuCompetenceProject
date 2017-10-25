using System.Collections.Generic;
using Gamelogic.Extensions;
using JetBrains.Annotations;
using UnityEngine;

public class AI : MonoBehaviour
{
    public PathFinding _PathFinding;
    public Vector3 _SpawnPosition;
    public float _WaitTime;

    // Test case - Can be used to set the AI to never return to home, even if the artefact is found
    public bool _ContiniouslySearchAndMove;

    [SerializeField, ReadOnly, UsedImplicitly]
    private string _currentStateLiteral;
    [HideInInspector]
    public HumanMotor _HumanMotor;
    [HideInInspector]
    public Inventory _Inventory;
    [HideInInspector]
    public GameObject _ItemToLookFor;
    [HideInInspector]
    public List<GameObject> _BlacklsitedNodes;
    private State _currentState;
    

    private void OnEnable()
    {
        GameManager.OnLevelGenerated += LevelGenerated;
    }

    private void OnDisable()
    {
        GameManager.OnLevelGenerated -= LevelGenerated;
    }

    private void Start()
    {
        _HumanMotor = GetComponent<HumanMotor>();
        _Inventory = GetComponent<Inventory>();
        _BlacklsitedNodes = new List<GameObject>();
    }

    private void LevelGenerated()
    {
        SetState(new SearchingState(this));
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Tick();
        }
    }

    public void SetState(State state)
    {
        if (_currentState != null)
        {
            _currentState.OnStateExit();
        }
        _currentState = state;
        _currentStateLiteral = state.ToString();

        if (_currentState != null)
        {
            _currentState.OnStateEnter();
        }
    }
}