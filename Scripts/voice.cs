using UnityEngine;
using SpeechLib;
using UnityEngine.UI;
using TMPro;

public class voice : MonoBehaviour
{
    //���� SpVoice ����
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
        //ʵ���� SpVoice ����
        Avoice = new SpVoice();
        //������������
        Avoice.Voice = Avoice.GetVoices(string.Empty, string.Empty).Item((int)sliderLanguage.value); // 0��ʾ�Ǻ��1234����ʾӢ����ǿ�����ͬ
        //�����ٶȣ���Χ-10��10��Ĭ����0
        Avoice.Rate = 0;
        //������������Χ0��100��Ĭ����100
        Avoice.Volume = 100;

    }

    //void Update()
    //{
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    //ͬ���ʶ�(ͬ���ʶ�ʱϵͳ��ͣ�����ֱ���ʶ���ϲŻ�����ִ�У�����ʹ���첽�ʶ�)
        //    Avoice.Speak("hello world");
        //    //�첽�ʶ�
        //    Avoice.Speak("̦ܿ��ʮ�ֻ���̦ܿ��ֲ��", SpeechVoiceSpeakFlags.SVSFlagsAsync);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    //��ͣʹ�øö���������ʶ����̣�ͬ���ʶ����޷�ʹ�ø÷�����ͣ
        //    Avoice.Pause();
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    //�ָ��ö�������Ӧ�ı���ͣ���ʶ�����
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
            hideBtnText.text = "����";
            splt = textArea.text.Split(new char[] { ',', '.','��','��' }, System.StringSplitOptions.RemoveEmptyEntries);
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
            hideBtnText.text = "����";
        }
        else if (!HideImg.activeSelf)
        {
            HideImg.SetActive(true);
            hideBtnText.text = "��ʾ";

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
