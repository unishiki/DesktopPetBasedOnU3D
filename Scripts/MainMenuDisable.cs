using UnityEngine;

public class MainMenuDisable : MonoBehaviour
{
    [SerializeField] private GameObject sizePage;
    private void OnEnable()
    {
        //UIScript.Instance.clearMemory();
        gameObject.AddComponent<WheelControl>();
    }
    private void OnDisable()
    {
        sizePage.SetActive(false);
        Destroy(GetComponent<WheelControl>());
        
    }


}
