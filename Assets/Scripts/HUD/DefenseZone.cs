using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DefenseTurretZone : MonoBehaviour
{
    public List<HealthBar> defenseZoneHealthBars;
    public List<DefenseTurret> turrets;
    public EnemySpawner relatedEnemySpawner;

    public TextMeshProUGUI waveText;

    bool _isPlayerInTrigger;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && DayNightController.isNight)
        {
            _isPlayerInTrigger = true;
            SetActiveStatusHealthBars(true);
            waveText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            SetActiveStatusHealthBars(false);
            waveText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(_isPlayerInTrigger)
        {
            for(int i = 0; i < defenseZoneHealthBars.Count; i++)
            {
                defenseZoneHealthBars[i]?.SetHealth(turrets[i].currentHealth, turrets[i].MaxHealth);
                waveText.text = $"Wave \n {relatedEnemySpawner.actualWaveNumber} / {relatedEnemySpawner.numberOfWaves}";
            }
        }
    }

    void SetActiveStatusHealthBars(bool status)
    {
        foreach(HealthBar healthBar in defenseZoneHealthBars)
        {
            healthBar.gameObject.SetActive(status);
        }
    }
}