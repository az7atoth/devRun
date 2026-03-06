using DevRun;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CheatController : MonoBehaviour
{
    private const string DEBUG_MESSAGE = "<color=#c63131>Cheat Controller</color>";


    [SerializeField] private InputActionReference _godModeActionReference;

    private PlayerController _playerController;

    [Inject]
    public void Construct(PlayerController playerController)
    {
        _playerController = playerController;
    }

    private void Subscribe()
    {
        _godModeActionReference.action.performed += ToggleGodMode;
    }

    private void Unsubscribe()
    {
        _godModeActionReference.action.performed -= ToggleGodMode;
    }

    private void ToggleGodMode(InputAction.CallbackContext obj)
    {
        _playerController.GodMode = !_playerController.GodMode;
        Debug.Log($"{DEBUG_MESSAGE}: God Mode is: {_playerController.GodMode}");
    }

    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
