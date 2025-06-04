using UnityEngine;

[System.Serializable]
public class PlayerConfig
{
    [Header("Info")]
    [SerializeField] private string playerName;

    [Header("Move")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("Combat")]
    [SerializeField] private float weaponRange = 6f;
    [SerializeField] private float weaponRotationSpeed = 100f;

    public string PlayerName => playerName;
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float WeaponRange => weaponRange;
    public float WeaponRotationSpeed => weaponRotationSpeed;
}
