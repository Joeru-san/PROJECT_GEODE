using System.Collections.Generic;
using UnityEngine;

// QuestManager.cs
public class QuestManager : MonoBehaviour
{
    public string currentQuestName;

    public static QuestManager inst;
    public List<QuestBaseObject> questArray;
    public int _currentQuestIndex = 0;
    public QuestBaseObject currentQuest;

    public GameObject playerReference;

    public QuestStateMachine stateMachine { get; set; }

    // Maps each ScriptableObject to its live state instance
    private Dictionary<QuestBaseObject, QuestBaseState> questStateMap;

    void Awake()
    {
        if (inst != null && inst != this) { Destroy(gameObject); return; }
        inst = this;
        DontDestroyOnLoad(gameObject);

        stateMachine = new QuestStateMachine();

        // Build the map: every quest object gets its matching state
        questStateMap = new Dictionary<QuestBaseObject, QuestBaseState>();
        foreach (var questObject in questArray)
            questStateMap[questObject] = questObject.CreateState(this, stateMachine);
    }

    void Start()
    {
        StartQuest(questArray[_currentQuestIndex]);
    }

    void Update()
    {
        if (currentQuest == null) return;
        stateMachine.currentQuestState?.QuestUpdate();
        currentQuestName = currentQuest.name;
    }

    // Call this to start any quest by passing its ScriptableObject
    public void StartQuest(QuestBaseObject questObject)
    {
        currentQuest = questObject;
        stateMachine.Initialize(questStateMap[questObject]);
    }

    public void ChooseNextState()
    {
        _currentQuestIndex++;
        if (_currentQuestIndex < questArray.Count)
        {
            currentQuest = questArray[_currentQuestIndex];
            stateMachine.ChangeState(questStateMap[currentQuest]); // just this, no null assignment before
        }
        else
        {
            stateMachine.currentQuestState = null;
            Debug.Log("All quests completed!");
        }
    }

    public void JumpToQuest(int index)
    {
        _currentQuestIndex = index;
        currentQuest = questArray[_currentQuestIndex];
        stateMachine.ChangeState(questStateMap[currentQuest]);
    }
}
