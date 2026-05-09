using UnityEngine;

[CreateAssetMenu(fileName = "NewGoToPointQuest", menuName = "Quests/GoToPointQuest")]
public class GoToPointObject : QuestBaseObject
{
    public Vector3 pointToReach;

    public override QuestBaseState CreateState(QuestManager qm, QuestStateMachine sm)
        => new GoToPointQuest(qm, sm);
}