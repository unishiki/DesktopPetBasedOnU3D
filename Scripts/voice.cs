using UnityEngine;
using SpeechLib;
using UnityEngine.UI;
using TMPro;

public class voice : MonoBehaviour
{
    //声明 SpVoice 对象
    SpVoice Avoice;
    [SerializeField] private TMP_InputField textArea;
    [SerializeField] private TMP_InputField textArea_Key;
    [SerializeField] private TextMeshProUGUI hideBtnText;
    [SerializeField] string[] splt;
    [SerializeField] private GameObject HideImg;
    [SerializeField] private Slider slider;
    [SerializeField] private Slider sliderLanguage;
    int nowNum = 0;
    [SerializeField]string nowList;
    void Start()
    {
        //实例化 SpVoice 对象
        Avoice = new SpVoice();
        //管理语音属性
        Avoice.Voice = Avoice.GetVoices(string.Empty, string.Empty).Item((int)sliderLanguage.value); // 0表示是汉语，1234都表示英语，就是口音不同
        //语音速度，范围-10到10，默认是0
        Avoice.Rate = 0;
        //语音音量，范围0到100，默认是100
        Avoice.Volume = 100;

    }

    //void Update()
    //{
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    //同步朗读(同步朗读时系统会停在这里，直到朗读完毕才会往下执行，建议使用异步朗读)
        //    Avoice.Speak("hello world");
        //    //异步朗读
        //    Avoice.Speak("芸苔是十字花科芸苔属植物", SpeechVoiceSpeakFlags.SVSFlagsAsync);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    //暂停使用该对象的所有朗读进程，同步朗读下无法使用该方法暂停
        //    Avoice.Pause();
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    //恢复该对象所对应的被暂停的朗读进程
        //    Avoice.Resume();
        //}
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    startRead();
        //}
   // }

    public void Renew()
    {
        try
        {
            textArea_Key.gameObject.SetActive(false);
            Avoice.Voice = Avoice.GetVoices(string.Empty, string.Empty).Item((int)sliderLanguage.value);
            HideImg.SetActive(false);
            hideBtnText.text = "隐藏";
            splt = textArea.text.Split(new char[] { ',', '.','，','。' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splt.Length - 1; i++)
            {
                int rdmNum = Random.Range(0, splt.Length - 1);
                if (rdmNum != i)
                {
                    string tempWord = splt[i];
                    splt[i] = splt[rdmNum];
                    splt[rdmNum] = tempWord;
                }
            }
            nowNum = 0;
            nowList = "";
            for (int i = 0; i < splt.Length; i++)
            {
                nowList += splt[i] + ",";
            }
        }
        catch
        {
            Debug.Log("awake error");
            return;
        }
       
    }
    public void startRead()
    {
        if (splt.Length == 0) return;
        try
        {
            Avoice.Rate = (int)slider.value;
            Avoice.Voice = Avoice.GetVoices(string.Empty, string.Empty).Item((int)sliderLanguage.value);

            Avoice.Speak(splt[nowNum], SpeechVoiceSpeakFlags.SVSFlagsAsync);
            Avoice.Speak(splt[nowNum], SpeechVoiceSpeakFlags.SVSFlagsAsync);
            nowNum++;
        }
        catch
        {
            Debug.Log("start error");
            return;
        }
        
        
    }
    public void hideBtn()
    {
        if (HideImg.activeSelf)
        {
            HideImg.SetActive(false);
            hideBtnText.text = "隐藏";
        }
        else if (!HideImg.activeSelf)
        {
            HideImg.SetActive(true);
            hideBtnText.text = "显示";

        }
        
    }

    public void Repeat()
    {
        if (splt.Length == 0) return;
        try
        {
            Avoice.Voice = Avoice.GetVoices(string.Empty, string.Empty).Item((int)sliderLanguage.value);
            Avoice.Rate = (int)slider.value;
            Avoice.Speak(splt[nowNum - 1], SpeechVoiceSpeakFlags.SVSFlagsAsync);
            Avoice.Speak(splt[nowNum - 1], SpeechVoiceSpeakFlags.SVSFlagsAsync);
            return;
        }
        catch
        {
            Debug.Log("repeat error");
            return;
        }
        
    }
    public void key()
    {
        try
        {
            textArea_Key.gameObject.SetActive(!textArea_Key.gameObject.activeSelf);
            textArea_Key.text = "";
            textArea_Key.text = nowList.Remove(nowList.Length - 1, 1);
            if (HideImg.activeSelf) HideImg.SetActive(false);
        }
        catch
        {
            return;
        }
       
    }
}
