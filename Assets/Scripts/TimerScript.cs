using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private Image imageTimer;
    [SerializeField] private float MaxTime;
    private float currentTime;

    public bool create;

    void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CreatingTimer();
    }

    private void CreatingTimer()
    {
        create = false;
        currentTime += Time.deltaTime;
        if (currentTime >= MaxTime)
        {
            create = true;
            currentTime = 0;
        }

        imageTimer.fillAmount = currentTime / MaxTime;
    }



}
