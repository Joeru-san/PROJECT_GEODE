using DG.Tweening;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public BaseItem itemType;
    public TextMeshPro toolTip;
    public int amount {get; private set;} = 0;

    private bool _isTooltipVisible = false;

    Vector3 _initialScale = Vector3.zero;

    void Awake()
    {
        this.amount = Random.Range(itemType.minSpawnAmount, itemType.maxSpawnAmount);

        toolTip.text = "x" + this.amount + " " + itemType.name;

        if (toolTip != null)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        _initialScale = toolTip.gameObject.transform.localScale;
    }

    public void ShowTooltip()
    {
        if (toolTip != null && !_isTooltipVisible)
        {
            _isTooltipVisible = true;
            toolTip.gameObject.SetActive(true);

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(toolTip.gameObject.transform.DOScaleY(_initialScale.y / 4, 0.2f));
            mySequence.Append(toolTip.gameObject.transform.DOScaleY(_initialScale.y * 2, 0.2f));
            mySequence.Append(toolTip.gameObject.transform.DOScaleY(_initialScale.y, 0.5f));    

        }
    }

    public void HideTooltip()
    {
        if (toolTip != null && _isTooltipVisible)
        {
            _isTooltipVisible = false;
            toolTip.gameObject.SetActive(false);
        }
    }

    public void DestroySelf()
    {
        gameObject.layer = 0;
        gameObject.transform.DOScale(0, 0.2f);
        this.DOKill();
        Destroy(gameObject, 0.7f);
    }
}
