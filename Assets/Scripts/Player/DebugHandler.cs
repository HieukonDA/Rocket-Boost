using UnityEngine;
using UnityEngine.InputSystem;

public class DebugHandler
{
    private readonly ILevelManager levelManager;
    private bool isCollisionable = true;

    public DebugHandler(ILevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                levelManager.OnLevelComplete();
            }
            else if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                isCollisionable = !isCollisionable;
            }
        }
    }

    public bool IsCollisionable()
    {
        return isCollisionable;
    }
}