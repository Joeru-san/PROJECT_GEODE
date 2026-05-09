using UnityEngine;

public class GoToPointQuest : QuestBaseState
{
	public GoToPointQuest(QuestManager questManager, QuestStateMachine questStateMachine) : base(questManager, questStateMachine) {}

    public override void InitQuest()
    {
        base.InitQuest();

        QuestManager.inst.currentQuest.actualQuestState = QuestState.OnGoing;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} initiated");
    }

    public override void QuestUpdate()
    {
        base.QuestUpdate();
        GoToPointObject obj = QuestManager.inst.currentQuest as GoToPointObject;

        float dist = Vector2.Distance(QuestManager.inst.playerReference.transform.position, obj.pointToReach);

        if (dist < 1.5f)
        {
            EndQuest();
        }
    }

    public override void EndQuest()
    {
        base.EndQuest();
        QuestManager.inst.currentQuest.actualQuestState = QuestState.Complete;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} ended");

        QuestManager.inst.ChooseNextState(); // automatically starts next
    }
}