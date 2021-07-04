using System;
using UnityEngine;

[Flags]
// use power of 2 in enum values to make flags work
public enum GameState
{
    Gameplay = 1, // regular state: player moves, attacks, can perform actions
    Pause = 2, // pause menu is opened, the whole game world is frozen
    Inventory = 4, //when inventory UI or cooking UI are open
    Dialogue = 8,
    Cutscene = 16,

    // when the character steps into LocationExit trigger, fade to black begins and control is removed from the player
    LocationTransition = 32,

    //enemy is nearby and alert, player can't open Inventory or initiate dialogues, but can pause the game
    Combat = 64,
}

//[CreateAssetMenu(fileName = "GameState", menuName = "Gameplay/GameState", order = 51)]
public class GameStateManagerSO : ScriptableObject
{
    [SerializeField] private GameStateInstanceSO[] _states = default;

    private GameStateInstanceSO _currentGameState = default;
    private GameStateInstanceSO _previousGameState = default;
    public GameState CurrentGameState => _currentGameState.state;

    public void UpdateGameState(GameState newGameState)
    {
        GameStateInstanceSO newStateSO = Array.Find(_states, x => x.state == newGameState);

        // no such state in states array
        if (!newStateSO) return;

        // first time state assignation
        if (!_currentGameState)
        {
            _currentGameState = newStateSO;
            _currentGameState.OnStateEnter();
            return;
        }

        // don't transfer from and to same state
        if (newStateSO.state == _currentGameState.state) return;

        // check if we can transfer from and to
        if (!newStateSO.CanTransitionFrom(_currentGameState.state) || !_currentGameState.CanTransitionTo(newGameState))
            return;

        _currentGameState.OnStateExit();
        newStateSO.OnStateEnter();

        _currentGameState = newStateSO;
        _previousGameState = _currentGameState;
    }

    public void ResetToPreviousGameState()
    {
        if (_previousGameState) UpdateGameState(_previousGameState.state);
    }
}