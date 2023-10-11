using UnityEngine;

public class TowardMousePos : MonoBehaviour
{
    [SerializeField] private Transform neck;
    [SerializeField] private Animator anim;

    private void Update()
    {
        if (MouseEvents.Instance.dragModle) return;
        // 设置分辨率为1200:900可以计算出鼠标当前位置
        float x = Mathf.Clamp((Input.mousePosition.x - 1200 * .5f) / Screen.width * .5f * 180, -50, 50);
        float y = Mathf.Clamp((Input.mousePosition.y - 900 * .5f) / Screen.height * .5f * 180, -20, 50);
        // 旋转脖子角度
        if (!anim.GetBool("exit") && !anim.GetBool("bow"))
            neck.transform.rotation = Quaternion.Euler(-y + 45, -x, 0); // 上下，左右

    }
}
