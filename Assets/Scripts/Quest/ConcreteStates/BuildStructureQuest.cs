using UnityEngine;

public class BuildStructureQuest : QuestBaseState
{
    BuildStructureObject obj;

    public BuildStructureQuest(QuestManager questManager, QuestStateMachine stateMachine) 
        : base(questManager, stateMachine) { }

    public override void InitQuest()
    {
        base.InitQuest();
        obj = QuestManager.inst.currentQuest as BuildStructureObject;
        obj.actualQuestState = QuestState.OnGoing;
        Debug.Log($"{obj.QuestName} quest of type {GetType().Name} initiated");
        ShopUI.OnStructureBuild += EndQuest;
    }

    public override void EndQuest()
    {
        ShopUI.OnStructureBuild -= EndQuest;
        base.EndQuest();
        obj.actualQuestState = QuestState.Complete;
        Debug.Log($"{obj.QuestName} quest of type {GetType().Name} ended");
        QuestManager.inst.ChooseNextState();
    }
}