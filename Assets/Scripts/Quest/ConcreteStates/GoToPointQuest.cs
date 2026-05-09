using System;
using UnityEngine;

public class GoToPointQuest : QuestBaseState
{
	public GoToPointQuest(QuestManager questManager, QuestStateMachine questStateMachine) : base(questManager, questStateMachine) {}

    public static Action<Vector3> GoToNewPoint;

    GoToPointObject obj;

    public override void InitQuest()
    {
        base.InitQuest();

        obj = QuestManager.inst.currentQuest as GoToPointObject;

        GoToNewPoint?.Invoke(obj.pointToReach);

        obj.actualQuestState = QuestState.OnGoing;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} initiated");
    }

    public override void QuestUpdate()
    {
        if (isEnding) return; // critical — stops re-entry
        base.QuestUpdate();
    
        float dist = Vector2.Distance(QuestManager.inst.playerReference.transform.position, obj.pointToReach);

        if (dist < 1.5f)
        {
            EndQuest();
        }
    }

    public override void EndQuest()
    {
        base.EndQuest();
        obj.actualQuestState = QuestState.Complete;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} ended");

        QuestManager.inst.ChooseNextState();
    }
}