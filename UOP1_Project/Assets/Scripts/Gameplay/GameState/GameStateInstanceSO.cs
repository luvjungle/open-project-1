using UnityEngine;

[CreateAssetMenu]
public class GameStateInstanceSO : ScriptableObject
{
    public GameState state = default;
    [SerializeField] private GameState _canTransitionTo = default;
    [SerializeField] private GameState _canTransitionFrom = default;

    public bool CanTransitionTo(GameState newState)
    {
        return _canTransitionTo.HasFlag(newState);
    }
    
    public bool CanTransitionFrom(GameState lastState)
    {
        return _canTransitionFrom.HasFlag(lastState);
    }

    public void OnStateEnter() { }

    public void OnStateExit() { }
}