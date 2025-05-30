using UnityEngine;

public abstract class BaseState<TController,TFactory> 
    where TController : MonoBehaviour
    where TFactory : class
{
    protected TController _controller;
    protected TFactory _factory;

    public BaseState(TController controller, TFactory factory)
    {
        _controller = controller;
        _factory = factory;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Exit();
    public abstract void CheckSwitchState();
}