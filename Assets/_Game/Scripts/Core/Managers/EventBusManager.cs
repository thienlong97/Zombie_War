using System.Collections.Generic;
using System;
using UnityEngine;

public enum EventType
{
    PlayerDied,
    PlayerFired,
    PlayerHit,
    PlayerGrabWeapon,
    EnemyDied,
    EnemyHit,
    EnemyOutOfRange,
    LevelStarted,
    LevelCompleted,
    LevelFailed
}

public class EventBusManager : GenericSingleton<EventBusManager>
{
    private readonly Dictionary<EventType, Action<object>> _eventTable = new();

    public void Subscribe(EventType eventType, Action<object> listener)
    {
        if (_eventTable.TryGetValue(eventType, out var existingListeners))
        {
            _eventTable[eventType] = existingListeners + listener;
        }
        else
        {
            _eventTable[eventType] = listener;
        }
    }

    public void Unsubscribe(EventType eventType, Action<object> listener)
    {
        if (_eventTable.TryGetValue(eventType, out var existingListeners))
        {
            existingListeners -= listener;
            if (existingListeners == null)
                _eventTable.Remove(eventType);
            else
                _eventTable[eventType] = existingListeners;
        }
    }

    public void Publish(EventType eventType, object eventData = null)
    {
        if (_eventTable.TryGetValue(eventType, out var listeners))
        {
            listeners?.Invoke(eventData);
        }
    }
}
