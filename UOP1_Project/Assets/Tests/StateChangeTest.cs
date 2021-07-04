using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StateChangeTest
{
    private GameStateManagerSO _stateManagerSo;
        
    [SetUp]
    public void Setup()
    {
        var gameStateArray = new GameStateInstanceSO[Enum.GetNames(typeof(GameState)).Length];
        int count = 0;
        foreach (var stateInstance in Enum.GetValues(typeof(GameState)))
        {
            var instance = ScriptableObject.CreateInstance<GameStateInstanceSO>();
            instance.state = (GameState) stateInstance;
            instance.GetType().GetField("_canTransitionFrom", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(instance, -1);
            instance.GetType().GetField("_canTransitionTo", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(instance, -1);
            
            if (instance.state == GameState.Dialogue)
                instance.GetType().GetField("_canTransitionFrom", BindingFlags.NonPublic | BindingFlags.Instance)?
                    .SetValue(instance,
                        GameState.Combat | GameState.Dialogue | GameState.Gameplay | GameState.Inventory |
                        GameState.Pause | GameState.LocationTransition);

            gameStateArray[count] = instance;
            count++;
        }

        _stateManagerSo = ScriptableObject.CreateInstance<GameStateManagerSO>();
        _stateManagerSo.GetType().GetField("_states", BindingFlags.NonPublic | BindingFlags.Instance)?
            .SetValue(_stateManagerSo, gameStateArray);
    }
    
    [Test]
    public void SetFirstState()
    {
        _stateManagerSo.UpdateGameState(GameState.Combat);
        Assert.AreEqual(GameState.Combat, _stateManagerSo.CurrentGameState);
    }
    
    [Test]
    public void ChangeState()
    {
        _stateManagerSo.UpdateGameState(GameState.Combat);
        _stateManagerSo.UpdateGameState(GameState.Cutscene);
        Assert.AreEqual(GameState.Cutscene, _stateManagerSo.CurrentGameState);
    }
    
    [Test]
    public void StateCantBeChanged()
    {
        _stateManagerSo.UpdateGameState(GameState.Cutscene);
        _stateManagerSo.UpdateGameState(GameState.Dialogue);
        Assert.AreEqual(GameState.Cutscene, _stateManagerSo.CurrentGameState);
    }

    [TearDown]
    public void TearDown()
    {
        ScriptableObject.Destroy(_stateManagerSo);
    }
}