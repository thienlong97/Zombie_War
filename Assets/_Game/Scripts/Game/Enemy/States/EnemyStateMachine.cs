using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateType
{
    Idle,
    Chase,
    Attack,
    Die,
    Patrol
}


public class EnemyStateMachine
{
    private EnemyStateBase currentState;
    private readonly Dictionary<EnemyStateType, EnemyStateBase> states = new Dictionary<EnemyStateType, EnemyStateBase>();
    public EnemyStateBase CurrentState => currentState;

    public void Initialize(EnemyStateType startState)
    {
        if (states.TryGetValue(startState, out EnemyStateBase state))
        {
            currentState = state;
            currentState.Enter();
        }
        else
        {
            Debug.LogError($"State {startState} not found in state machine!");
        }
    }

    public void AddState(EnemyStateType stateType, EnemyStateBase state)
    {
        states[stateType] = state;
    }

    public void ChangeState(EnemyStateType newStateType)
    {
        if (states.TryGetValue(newStateType, out EnemyStateBase newState))
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            Debug.LogError($"State {newStateType} not found in state machine!");
        }
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
} 