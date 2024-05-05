using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public int stopTimer = 1;
    public Text healthFull;
    public float currentTime, maxTime = 5.0f;

    private void Update()
    {
        if (currentTime <= 5 && stopTimer == 0)
            currentTime += Time.deltaTime;
        else 
        {
            healthFull.text = ""; // burasý geliþtirilebilir bu öðeyi burda tutmak yerine yok edip yeniden yaratma gibi bir method yapýlabilir
            currentTime = 0;
            stopTimer = 1;
        }
            
    }
    public void annen()
    {
        healthFull.text = "Your health is full";
        if (stopTimer == 1)
        {
            stopTimer = 0;
            Update();
        }
         


    }
}
