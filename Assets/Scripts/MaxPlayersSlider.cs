using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxPlayersSlider : MonoBehaviour {

    public Slider slider;
    public Text currentPlayersText;

    private void Start()
    {
        currentPlayersText.text = slider.value.ToString();
        slider.onValueChanged.AddListener((arg0) =>
        {
            currentPlayersText.text = slider.value.ToString();
        });
    }
}
