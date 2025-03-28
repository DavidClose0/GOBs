using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string name;
    public Dictionary<Goal, float> effectsOnGoals;

    public Action(string name)
    {
        this.name = name;
        effectsOnGoals = new Dictionary<Goal, float>();
    }

    // Add effect values to goals
    public void perform()
    {
        foreach (var effect in effectsOnGoals)
        {
            effect.Key.value += effect.Value;
        }
    }
}
