using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildStructureQuest", menuName = "Quests/BuildStructureQuest")]
public class BuildStructureObject : QuestBaseObject
{
    public override QuestBaseState CreateState(QuestManager qm, QuestStateMachine sm)
        => new BuildStructureQuest(qm, sm);   
}