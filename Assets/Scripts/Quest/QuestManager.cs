using System;
using System.Collections.Generic;
using UnityEngine;

// QuestManager.cs
public class QuestManager : MonoBehaviour
{
    public string currentQuestName;

    public static Action OnAllQuestEnded;

    public static QuestManager inst;
    public List<QuestBaseObject> questArray;
    public int _currentQuestIndex = 0;
    public QuestBaseObject currentQuest;

    public GameObject playerReference;

    public QuestStateMachine stateMachine { get; set; }

    [SerializeField] DayNightController dayNightController;

    // Use a List instead of a Dictionary to allow duplicate quests
    private List<QuestBaseState> questStates;

    void Awake()
    {
        if (inst != null && inst != this) { Destroy(gameObject); return; }
        inst = this;
        DontDestroyOnLoad(gameObject);

        stateMachine = new QuestStateMachine();

        // Build the list: every quest object gets its matching state at the same index
        questStates = new List<QuestBaseState>();
        foreach (var questObject in questArray)
        {
            questStates.Add(questObject.CreateState(this, stateMachine));
        }
        
        dayNightController.gameObject.SetActive(false);
    }

    void Start()
    {
        StartQuest(_currentQuestIndex);
    }

    void Update()
    {
        if (currentQuest == null) return;
        stateMachine.currentQuestState?.QuestUpdate();
        currentQuestName = currentQuest.name;
    }

    // Pass the index instead of the ScriptableObject
    public void StartQuest(int index)
    {
        _currentQuestIndex = index;
        currentQuest = questArray[_currentQuestIndex];
        stateMachine.Initialize(questStates[_currentQuestIndex]);
    }

    public void ChooseNextState()
    {
        _currentQuestIndex++;
        if (_currentQuestIndex < questArray.Count)
        {
            currentQuest = questArray[_currentQuestIndex];
            stateMachine.ChangeState(questStates[_currentQuestIndex]);
        }
        else
        {
            stateMachine.currentQuestState = null;
            Debug.Log("All quests completed!");
            OnAllQuestEnded?.Invoke();
        }
    }

    public void JumpToQuest(int index)
    {
        _currentQuestIndex = index;
        currentQuest = questArray[_currentQuestIndex];
        stateMachine.ChangeState(questStates[_currentQuestIndex]);
    }
}
