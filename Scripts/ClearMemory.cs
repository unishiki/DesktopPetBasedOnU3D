using UnityEngine;

// 只在工作模式时存在
public class ClearMemory : MonoBehaviour
{
    private float timer;

    private void Awake()
    {
        timer = 0;
    }
    private void FixedUpdate() // 释放内存
    {

        timer += Time.fixedDeltaTime;

        if (timer > 60f)
        {
            timer = 0;
            UIScript.Instance.clearMemory();
        }
    }

    private void OnDestroy()
    {
        UIScript.Instance.clearMemory();
    }
}
