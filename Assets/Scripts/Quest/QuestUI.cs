using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questNameText;
    [SerializeField] TextMeshProUGUI questDescriptionText;

    void Awake()
    {
        QuestBaseState.OnQuestCompleted += ChangeQuestTexts;
        QuestManager.OnTutorialEnded += BasicQuestText;
    }

    void ChangeQuestTexts()
    {
        questNameText.text = QuestManager.inst.currentQuest.QuestName;
        questDescriptionText.text = QuestManager.inst.currentQuest.QuestDescription;
    }

    void BasicQuestText()
    {
        if(QuestManager.inst.stateMachine.currentQuestState == null)
        {
            questNameText.text = "Tutorial completed";
            questDescriptionText.text = "Explore the map and escape from the Geode";
        }
    }
}
