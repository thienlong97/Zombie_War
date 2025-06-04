using System;
using UnityEngine;



//public class Joystick
//{
//    public Vector2 Direction;
//}

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private JoystickBase joyStick;


    public event Action OnSwitchWeapon;
    public event Action OnThrowBomb;
    public Vector3 JoystickDirection => joyStick.Direction;
    public bool IsDragging  => JoystickDirection.sqrMagnitude > 0.01f;

    public void UI_SwitchWeapon() => OnSwitchWeapon?.Invoke();
    public void UI_ThrowBomb() => OnThrowBomb?.Invoke();

}
