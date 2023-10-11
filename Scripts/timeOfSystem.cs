using TMPro;
using UnityEngine;
using System;

public class timeOfSystem : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private int hour;
    private int minute;
    private int second;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        hour = DateTime.Now.Hour;
        minute = DateTime.Now.Minute;
        second = DateTime.Now.Second;

        tmp.text = hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2");

    }
}
