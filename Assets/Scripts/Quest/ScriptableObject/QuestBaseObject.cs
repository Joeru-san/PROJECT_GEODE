using UnityEngine;

public enum QuestState
{
    ToStart,
    OnGoing,
    Complete
}

public abstract class QuestBaseObject : ScriptableObject
{
    public string QuestName;
    
    [TextArea(15,20)]
    public string QuestDescription;
    public QuestState actualQuestState = QuestState.ToStart;

    // Each subclass knows which state to instantiate
    public abstract QuestBaseState CreateState(QuestManager qm, QuestStateMachine sm);

    void Awake()
    {
        actualQuestState = QuestState.ToStart;
    }
}