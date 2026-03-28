using DG.Tweening;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour, IPoolable
{
    public BaseItem scriptableObjectType;
    public TextMeshPro toolTip;
    public int amount {get; private set;} = 0;

    private bool _isTooltipVisible = false;

    Vector3 _initialToolTipScale = Vector3.zero;
    Vector3 _initialObjectScale = Vector3.zero;

    void Awake()
    {
        this.amount = Random.Range(scriptableObjectType.minSpawnAmount, scriptableObjectType.maxSpawnAmount + 1);

        _initialToolTipScale = toolTip.gameObject.transform.localScale;
        _initialObjectScale = gameObject.transform.localScale;

        if (scriptableObjectType.itemMesh != null)
        {
            GetComponent<MeshFilter>().sharedMesh = scriptableObjectType.itemMesh;
        }
        
        if (scriptableObjectType.itemMaterial != null)
        {
            GetComponent<Renderer>().material = scriptableObjectType.itemMaterial;
        }

        toolTip.text = "x" + this.amount + " " + scriptableObjectType.name;


        if (toolTip != null)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    // Call this after SpawnFromPool to override the random amount with a known value.
    public void Initialize(int assignedAmount)
    {
        gameObject.layer = LayerMask.NameToLayer("ObjectLayer");
        gameObject.transform.DOScale(_initialObjectScale, 0.2f);
        this.amount = assignedAmount;
        if (toolTip != null)
            toolTip.text = "x" + this.amount + " " + scriptableObjectType.name;
    }

    public void ShowTooltip()
    {
        if (toolTip != null && !_isTooltipVisible)
        {
            _isTooltipVisible = true;
            toolTip.gameObject.SetActive(true);

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(toolTip.gameObject.transform.DOScaleY(_initialToolTipScale.y / 4, 0.2f));
            mySequence.Append(toolTip.gameObject.transform.DOScaleY(_initialToolTipScale.y * 2, 0.2f));
            mySequence.Append(toolTip.gameObject.transform.DOScaleY(_initialToolTipScale.y, 0.5f));    
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

    public void OnObjectDestroy()
    {
        if (gameObject.layer == 0) return; 
        gameObject.layer = 0;
        gameObject.transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack) 
            .OnComplete(() => 
            {
                ObjectPooler.inst.ReAddToPool(scriptableObjectType.name, this.gameObject);
            });
    }
}