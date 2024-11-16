using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateFromSlider : MonoBehaviour
{
    public Slider slider;
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void UpdateValue()
    {
        text.text = slider.value.ToString();
    }
}
