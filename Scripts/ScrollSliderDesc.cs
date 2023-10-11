using UnityEngine;
using UnityEngine.UI;

public class ScrollSliderDesc : MonoBehaviour
{
    private Slider slider;
    private float minValue;
    private float maxValue;

    private float f;
    private void Awake()
    {
        slider = transform.GetChild(1).GetComponent<Slider>();
        minValue = slider.minValue;
        maxValue = slider.maxValue;
        f = 0;
    }
    private void Update()
    {
        f = Input.GetAxis("Mouse ScrollWheel");
        if (slider.value >= slider.minValue && slider.value <= slider.maxValue)
        {
            slider.value += 30 * f;
            slider.value = slider.value <= minValue ? minValue : slider.value;
            slider.value = slider.value >= maxValue ? maxValue : slider.value;
        }
    }

}
