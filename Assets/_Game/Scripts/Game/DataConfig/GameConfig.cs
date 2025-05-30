using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig; 

    [Header("Weapon")]
    [SerializeField] private WeaponStrategy[] weaponStrategy;
    public AudioClip grabWeaponSfx;


    public WeaponStrategy[] WeaponStrategy => weaponStrategy;
    public PlayerConfig PlayerConfig => playerConfig;
}

