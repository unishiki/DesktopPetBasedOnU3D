using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    private float f = 0;
    private List<GameObject> btns;
    private GameObject setSizePage;
    private GameObject setOpacityPage;
    private GameObject setAllOpacityPage;

    private void Awake()
    {
        btns = UIScript.Instance.btns;
        setSizePage = UIScript.Instance.sizePage;
        setOpacityPage = UIScript.Instance.opacityPage;
        setAllOpacityPage = UIScript.Instance.setAllWindowsOpacitySliderPage;
    }
    void Update()
    {
        if (setSizePage.activeSelf || setOpacityPage.activeSelf || setAllOpacityPage.activeSelf)
            return;
        f = Input.GetAxis("Mouse ScrollWheel");
        if (f > 0 && btns[btns.Count - 1].activeSelf) // ¡ø
        {
            UIScript.Instance.goUp();
        }
        else if (f < 0 && btns[btns.Count - 2].activeSelf)
        {
            UIScript.Instance.goDown();
        }
    }
}
