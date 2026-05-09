using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questNameText;
    [SerializeField] TextMeshProUGUI questDescriptionText;


    void Awake()
    {
        QuestBaseState.OnQuestCompleted += ChangeQuestTexts;
    }

    void ChangeQuestTexts()
    {
        questNameText.text = QuestManager.inst.currentQuest.name;
        questDescriptionText.text = QuestManager.inst.currentQuest.QuestDescription;
    }
}
