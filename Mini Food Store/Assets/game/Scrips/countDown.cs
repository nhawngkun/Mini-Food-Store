using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class countDown : MonoBehaviour
{
    public Image timeimg;
    public float dur, currtime;
    private Coroutine timeCoroutine;
    private bool isRunning = false;

    public GameObject countdownPanel;

    private Customer customerRef;

    void Start()
    {
        currtime = dur;
        timeimg.fillAmount = 1f;

        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    public void SetCustomer(Customer customer)
    {
        customerRef = customer;
    }

    public void ShowCountdownPanel()
    {
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }
    }

    public void HideCountdownPanel()
    {
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    public void StartCountdown()
    {
        if (!isRunning)
        {
            ShowCountdownPanel();

            currtime = dur;
            timeimg.fillAmount = 1f;
            timeCoroutine = StartCoroutine(TimeIen());
            isRunning = true;
        }
    }

    public void StopCountdown()
    {
        if (isRunning && timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            isRunning = false;

            HideCountdownPanel();
        }
    }

    public void ResetCountdown()
    {
        currtime = dur;
        timeimg.fillAmount = 1f;
    }

    IEnumerator TimeIen()
    {
        while (currtime >= 0)
        {
            timeimg.fillAmount = Mathf.InverseLerp(0, dur, currtime);
            yield return new WaitForSeconds(1f);
            currtime--;

            if (currtime < 0)
            {
                if (customerRef != null)
                {
                    customerRef.HandleTimeout();
                }

                isRunning = false;

                HideCountdownPanel();
            }
        }
    }

    void Update()
    {
    }
}