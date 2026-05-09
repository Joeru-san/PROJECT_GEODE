public class QuestStateMachine
{
    public QuestBaseState currentQuestState {get; set;}

    public void Initialize(QuestBaseState startingState)
    {
        currentQuestState = startingState;
        currentQuestState.InitQuest();
    }

    public void ChangeState(QuestBaseState newState)
    {
        currentQuestState.EndQuest();
        currentQuestState = newState;
        currentQuestState.InitQuest();
    }
}
