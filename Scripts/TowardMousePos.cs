using UnityEngine;

public class TowardMousePos : MonoBehaviour
{
    [SerializeField] private Transform neck;
    [SerializeField] private Animator anim;

    private void Update()
    {
        if (MouseEvents.Instance.dragModle) return;
        // ���÷ֱ���Ϊ1200:900���Լ������굱ǰλ��
        float x = Mathf.Clamp((Input.mousePosition.x - 1200 * .5f) / Screen.width * .5f * 180, -50, 50);
        float y = Mathf.Clamp((Input.mousePosition.y - 900 * .5f) / Screen.height * .5f * 180, -20, 50);
        // ��ת���ӽǶ�
        if (!anim.GetBool("exit") && !anim.GetBool("bow"))
            neck.transform.rotation = Quaternion.Euler(-y + 45, -x, 0); // ���£�����

    }
}
