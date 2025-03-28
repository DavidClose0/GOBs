using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GOBAgent : MonoBehaviour
{
    List<Goal> goals;
    List<Action> actions;
    Action tick;
    float tickLength = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goals = new List<Goal>();
        goals.Add(new Goal("Eat", 4));
        goals.Add(new Goal("Sleep", 3));
        goals.Add(new Goal("Bathroom", 3));

        // "Tick": "Eat" +1, "Sleep" +1, "Bathroom" +1
        tick = new Action("Tick");
        tick.effectsOnGoals.Add(goals[0], 1);
        tick.effectsOnGoals.Add(goals[1], 1);
        tick.effectsOnGoals.Add(goals[2], 1);

        actions = new List<Action>();

        // "Eat snack": "Eat" -2
        actions.Add(new Action("Eat snack"));
        actions[0].effectsOnGoals.Add(goals[0], -2);

        // "Eat main meal": "Eat" -2
        actions.Add(new Action("Eat main meal"));
        actions[1].effectsOnGoals.Add(goals[0], -4);

        // "Sleep in bed": "Sleep" -4
        actions.Add(new Action("Sleep in bed"));
        actions[2].effectsOnGoals.Add(goals[1], -4);

        // "Drink soda": "Eat" -1, "Sleep" -1, "Bathroom" +2
        actions.Add(new Action("Drink soda"));
        actions[3].effectsOnGoals.Add(goals[0], -1);
        actions[3].effectsOnGoals.Add(goals[1], -1);
        actions[3].effectsOnGoals.Add(goals[2], 2);

        // "Visit bathroom": "Bathroom" -4
        actions.Add(new Action("Visit bathroom"));
        actions[4].effectsOnGoals.Add(goals[2], -4);

        LogGoals();
        Debug.Log("Eat, Sleep, and Bathroom will increase by 1 every " + tickLength + " seconds.");
        InvokeRepeating("ApplyTick", tickLength, tickLength);
        InvokeRepeating("PerformBestAction", tickLength, tickLength);
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void ApplyTick()
    {
        tick.perform();
        Debug.Log("Performed " + tick.name + ".");
        LogGoals();
    }

    void PerformBestAction()
    {
        Action bestAction = null;
        float bestDiscontentment = float.PositiveInfinity;

        foreach (var action in actions)
        {
            float discontentment = GetPredictedDiscontentment(action);
            if (discontentment < bestDiscontentment)
            {
                bestDiscontentment = discontentment;
                bestAction = action;
            }
        }

        bestAction.perform();
        Debug.Log("Performed " + bestAction.name + ".");
        LogGoals();
    }

    float GetPredictedDiscontentment(Action action)
    {
        float discontentment = 0;
        foreach (var goal in goals) {
            float predictedValue = goal.value;

            // Check if action affects goal
            if (action.effectsOnGoals.TryGetValue(goal, out float effectValue))
            {
                predictedValue += effectValue;
            }

            discontentment += goal.calculateDiscontentment(predictedValue);
        }

        return discontentment;
    }

    void LogGoals()
    {
        string goalString = string.Join(", ", goals.Select(goal => goal.name + ": " + goal.value));
        Debug.Log(goalString);
    }
}
