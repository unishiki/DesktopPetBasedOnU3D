using UnityEngine;

public class RandomAnimModeCameraPosition : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private TowardMousePos towardMp;
    [SerializeField] private Transform neck;
    private Animator anim;
    private int rdmNum;
    private int lastNum;

    private void Start()
    {
        anim = Player.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        towardMp.enabled = false;
        neck.transform.rotation = Quaternion.Euler(0, 0, 0);
        rdmNum = 0;
        lastNum = -1;
    }

    private void OnDisable()
    {
        towardMp.enabled = true;
        anim.SetBool("needResetPos", false);
        
    }
    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position - new Vector3(0.05764216f, -.6f, 0.2975143f - 3.7f), 0.1f);
    }

    private void Update()
    {
        if (anim.GetBool("needResetPos"))
        {
            //Player.transform.position = new Vector3(.06f, .2f, .3f);
            Player.transform.rotation = Quaternion.Euler(0, 4f, 0);
            Physics.SyncTransforms();
            anim.SetBool("needResetPos", false);
        }

        if (!anim.GetBool("rdm01") && !anim.GetBool("rdm02") && !anim.GetBool("rdm03") && !anim.GetBool("rdm04"))
        {
            rdmNum = Random.Range(1, 5); // 1/2/3/4
            while (lastNum == rdmNum)
            {
                rdmNum = Random.Range(1, 5);
            }
            lastNum = rdmNum;
            Debug.Log(rdmNum);
            switch (rdmNum)
            {
                case 1:
                    anim.SetBool("rdm01", true);
                    break;
                case 2:
                    anim.SetBool("rdm02", true);
                    break;
                case 3:
                    anim.SetBool("rdm03", true);
                    break;
                case 4:
                    anim.SetBool("rdm04", true);
                    break;
            }
        }
    }

}
