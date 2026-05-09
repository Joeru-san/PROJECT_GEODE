public class QuestStateMachine
{
    public QuestBaseState currentQuestState { get; set; }

    public void Initialize(QuestBaseState startingState)
    {
        currentQuestState = startingState;
        currentQuestState.InitQuest();
    }

    public void ChangeState(QuestBaseState newState)
    {
        // Don't call EndQuest here — the state already called it
        currentQuestState = newState;
        currentQuestState.InitQuest();
    }
}