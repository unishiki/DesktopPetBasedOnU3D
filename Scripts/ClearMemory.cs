using UnityEngine;

// ֻ�ڹ���ģʽʱ����
public class ClearMemory : MonoBehaviour
{
    private float timer;

    private void Awake()
    {
        timer = 0;
    }
    private void FixedUpdate() // �ͷ��ڴ�
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
