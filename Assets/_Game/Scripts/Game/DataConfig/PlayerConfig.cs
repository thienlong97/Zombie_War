using UnityEngine;

[System.Serializable]
public class PlayerConfig
{
    [Header("Info")]
    [SerializeField] private string playerName;

    [Header("Move")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    public string PlayerName => playerName;
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
}
