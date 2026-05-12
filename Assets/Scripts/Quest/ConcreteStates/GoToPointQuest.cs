using System;
using Unity.VisualScripting;
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
        if (isEnding) return;
        base.QuestUpdate();
    
        Collider[] hits = Physics.OverlapSphere(obj.pointToReach, 4f);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Player"))
            {
                EndQuest();
                break;
            }
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