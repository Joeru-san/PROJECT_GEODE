using UnityEngine;
using UnityEngine.UI;

public class SpawnerIcon : MonoBehaviour
{
    [SerializeField] Image refImage;
    [SerializeField] ObjectSpawner spawnerReference;

    [SerializeField] Sprite[] icons;


    void Update()
    {
        if(spawnerReference.isSpawning)
        {
            refImage.sprite = icons[0];
        }else
        {
            refImage.sprite = icons[1];
        }
    }
}
