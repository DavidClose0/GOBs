using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GOBAgent : MonoBehaviour
{
    List<Goal> goals;
    List<Action> actions;
    Action currentAction = null;
    Action tick;
    float tickLength = 5.0f;
    float actionTimer = 0;

    public TextMeshProUGUI statusText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goals = new List<Goal>();
        goals.Add(new Goal("Eat", 4));
        goals.Add(new Goal("Sleep", 3));
        goals.Add(new Goal("Bathroom", 3));

        // "Tick": "Eat" +1, "Sleep" +1, "Bathroom" +1
        tick = new Action("Tick", 0);
        tick.effectsOnGoals.Add(goals[0], 1);
        tick.effectsOnGoals.Add(goals[1], 1);
        tick.effectsOnGoals.Add(goals[2], 1);

        actions = new List<Action>();

        // "Eat snack": "Eat" -2
        actions.Add(new Action("Eat snack", 5));
        actions[0].effectsOnGoals.Add(goals[0], -2);

        // "Eat main meal": "Eat" -4
        actions.Add(new Action("Eat main meal", 10));
        actions[1].effectsOnGoals.Add(goals[0], -4);

        // "Sleep in bed": "Sleep" -4
        actions.Add(new Action("Sleep in bed", 10));
        actions[2].effectsOnGoals.Add(goals[1], -4);

        // "Drink soda": "Eat" -1, "Sleep" -1, "Bathroom" +2
        actions.Add(new Action("Drink soda", 5));
        actions[3].effectsOnGoals.Add(goals[0], -1);
        actions[3].effectsOnGoals.Add(goals[1], -1);
        actions[3].effectsOnGoals.Add(goals[2], 2);

        // "Visit bathroom": "Bathroom" -4
        actions.Add(new Action("Visit bathroom", 5));
        actions[4].effectsOnGoals.Add(goals[2], -4);

        LogGoals();
        Debug.Log("Each goal increases by 1 every " + tickLength + " seconds");
        InvokeRepeating("ApplyTick", tickLength, tickLength);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAction == null)
        {
            ChooseAction();
        }
        else
        {
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0)
            {
                currentAction.Perform();
                LogGoals();
                currentAction = null;
            }
        }

        UpdateStatusText();
    }

    void ApplyTick()
    {
        tick.Perform();
        LogGoals();
    }

    // Determine action with lowest predicted discontentment and set as current
    void ChooseAction()
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

        currentAction = bestAction;
        actionTimer = currentAction.duration;
        Debug.Log("Starting " + currentAction.name + ", Duration: " + currentAction.duration);
    }

    float GetPredictedDiscontentment(Action action)
    {
        float discontentment = 0;
        float tickFactor = action.duration / tickLength;

        foreach (var goal in goals) {
            float predictedValue = goal.value;

            // Check effect of action on goal
            if (action.effectsOnGoals.TryGetValue(goal, out float effectValue))
            {
                predictedValue += effectValue;
            }
            
            // Check effect of tick on goal
            if (tick.effectsOnGoals.TryGetValue(goal, out float tickEffectValue))
            {
                predictedValue += tickEffectValue * tickFactor;
            }

            discontentment += goal.CalculateDiscontentment(predictedValue);
        }

        return discontentment;
    }

    void LogGoals()
    {
        string goalString = string.Join(", ", goals.Select(goal => goal.name + ": " + goal.value));
        Debug.Log(goalString);
    }

    float GetTotalDiscontentment()
    {
        float totalDiscontentment = 0;
        foreach (var goal in goals)
        {
            totalDiscontentment += goal.GetDiscontentment();
        }
        return totalDiscontentment;
    }

    void UpdateStatusText()
    {
        string statusTextString = "";

        statusTextString += "Each goal increases by 1 every " + tickLength + " seconds\n";

        foreach (var goal in goals)
        {
            statusTextString += goal.name + ": " + goal.value + "\n";
        }

        statusTextString += "Discontentment: " + GetTotalDiscontentment() + "\n";

        statusTextString += "Performing " + currentAction.name + " in " + actionTimer.ToString("0.0") + " seconds\n";

        statusText.text = statusTextString;
    }
}
