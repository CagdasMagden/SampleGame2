using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class HealthManager : MonoBehaviour
{
    public int MaxHealth = 3;
    public int CurrentHealth;
    public Text healthText;

    void Start()
    {
        CurrentHealth = 2;
    }

    void Update()
    {
        if (CurrentHealth <= 0)
        {
            healthText.text = "Health Count: Dead!!!";
            //application kapacanak 
        }
        else
        healthText.text = "Health Count: " + CurrentHealth.ToString();
    }

    public void DamageCharacter()
    {
        CurrentHealth = CurrentHealth - 1;
    }
}
