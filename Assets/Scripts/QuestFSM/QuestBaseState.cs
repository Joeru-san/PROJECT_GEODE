public class QuestBaseState
{
    protected QuestManager quest;
    
    protected QuestStateMachine questStateMachine;

    public QuestBaseState(QuestManager quest, QuestStateMachine questStateMachine)
    {
        this.quest = quest;
        this.questStateMachine = questStateMachine;
    }

    protected bool isEnding = false;

    public virtual void InitQuest()
    {
        isEnding = false;
    }

    public virtual void QuestUpdate() { }

    public virtual void EndQuest()
    {
        isEnding = true;
    }
}
