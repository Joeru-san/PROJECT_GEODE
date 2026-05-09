public class QuestBaseState
{
    protected QuestManager quest;
    
    protected QuestStateMachine questStateMachine;


    public QuestBaseState(QuestManager quest, QuestStateMachine questStateMachine)
    {
        this.quest = quest;
        this.questStateMachine = questStateMachine;
    }

    public virtual void InitQuest() {}
    public virtual void EndQuest() {}
    public virtual void QuestUpdate() {}
}
