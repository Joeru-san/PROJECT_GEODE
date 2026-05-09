using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectItemQuest", menuName = "Quests/CollectItemQuest")]
public class CollectItemObject : QuestBaseObject
{
    public ItemType typeOfItemToCollect;
    public int numberOfItemsToCollect;

    public override QuestBaseState CreateState(QuestManager qm, QuestStateMachine sm)
        => new CollectItemQuest(qm, sm);   
}