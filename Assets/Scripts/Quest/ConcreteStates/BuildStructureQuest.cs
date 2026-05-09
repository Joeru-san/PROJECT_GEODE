using UnityEngine;

public class BuildStructureQuest : QuestBaseState
{
	public BuildStructureQuest(QuestManager questManager, QuestStateMachine stateMachine) : base(questManager, stateMachine) { }

    public override void InitQuest()
    {
        base.InitQuest();

        QuestManager.inst.currentQuest.actualQuestState = QuestState.OnGoing;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} initiated");
        ShopUI.OnStructureBuild += EndQuest;
    }

    public override void EndQuest()
    {
        base.EndQuest();
        QuestManager.inst.currentQuest.actualQuestState = QuestState.Complete;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} ended");

        QuestManager.inst.ChooseNextState(); // automatically starts next
    }
}