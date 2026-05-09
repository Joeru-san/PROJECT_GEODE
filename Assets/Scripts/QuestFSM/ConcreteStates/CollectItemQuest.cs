using UnityEngine;

public class CollectItemQuest : QuestBaseState
{
	public CollectItemQuest(QuestManager questManager, QuestStateMachine questStateMachine) : base(questManager, questStateMachine) { }

    public override void InitQuest()
    {
        base.InitQuest();
        
        QuestManager.inst.currentQuest.actualQuestState = QuestState.OnGoing;
        Debug.Log($"{QuestManager.inst.currentQuest.QuestName} quest of type {GetType().Name} initiated");
    }

    public override void QuestUpdate()
    {
        if (isEnding) return; // critical — stops re-entry
        base.QuestUpdate();

        if (QuestManager.inst.currentQuest == null)
        {
            Debug.LogError("currentQuest is null!");
            return;
        }

        CollectItemObject obj = QuestManager.inst.currentQuest as CollectItemObject;
        if (obj == null)
        {
            Debug.LogError("Current quest is not a CollectItemObject!");
            return;
        }

        if (PlayerInteraction.playerInventory == null)
        {
            Debug.LogError("playerInventory is null!");
            return;
        }

        if (PlayerInteraction.playerInventory.FindTotalItemAmount(obj.typeOfItemToCollect) >= obj.numberOfItemsToCollect)
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