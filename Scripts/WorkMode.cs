
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class WorkMode : MonoBehaviour
{
    private string[] url_voice;//�����ļ���
    private int currentID;
    private int urlLength;
    private string get_url;//�����ļ�����λ�á�����MP3����
    string Musicpath = "music.txt";
    private AudioClip myclip;//�����ļ�
    [SerializeField] private AudioSource AS;
    private double playTime;
    private bool isPause;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI nowTime;
    [SerializeField] private TextMeshProUGUI allTime;
    private int hour;
    private int second;
    private int minute;
    private int Ahour;
    private int Asecond;
    private int Aminute;

    private void OnEnable()
    {
        url_voice = new string[51];
        currentID = 0;
        urlLength = 0;
        playTime = 0;
        isPause = false;

        ArrayList arrlist = LoadFile(Environment.CurrentDirectory + "\\Music", Musicpath); // \\Assets\\Music

        string lines;
        foreach (string str in arrlist)
        {
            lines = str;
            url_voice[urlLength] = lines;
            urlLength++;
            //Debug.Log("��������ʲô" + lines);
        }
        //�ⲿ����������·����ƴ��www�����ļ�·��
        get_url = "file://" + Environment.CurrentDirectory + "//Music//" + url_voice[0] + ".mp3"; // //Assets//Music//

        StartCoroutine("DownloadVoice");
    }
    public void beforeBtn()
    {
        //AS.Stop();
        playTime = 0;
        isPause = false;
        if (currentID == 0)
        {
            currentID = urlLength - 1;
        }
        else
        {
            currentID--;
        }
        get_url = "file://" + Environment.CurrentDirectory + "//Music//" + url_voice[currentID] + ".mp3"; // //Assets//Music//
        StartCoroutine("DownloadVoice");
    }
    public void afterBtn()
    {
        playTime = 0;
        //AS.Stop();
        isPause = false;
        if (currentID == urlLength - 1)
        {
            currentID = 0;
        }
        else
        {
            currentID++;
        }
        get_url = "file://" + Environment.CurrentDirectory + "//Music//" + url_voice[currentID] + ".mp3"; // //Assets//Music//
        StartCoroutine("DownloadVoice");
    }
    public void stop()
    {
        if (AS.loop)
        {
            playTime = 0;
            StartCoroutine("DownloadVoice");
            isPause = false;
        }
        else if(!AS.loop)
        {
            afterBtn();
        }
    }

    public void pause()
    {
        isPause = true;
        AS.Pause();
    }
    public void play()
    {
        if(AS.isPlaying)
        {
            isPause = false;
            playTime = 0;
            AS.Play();
        }
        else if(!AS.isPlaying)
        {
            AS.Play();
            isPause = false;
        }
        
        
        
    }

    //ִ��Э�ɺ��� ���ҷ���ʱ��
    private IEnumerator AudioPlayFinished(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        //����������Ϻ�֮�����µĴ���
        #region   ����������ɺ�ִ�еĴ���
        
        #endregion
    }

    //c#��ȡ�ļ��ķ���
    public static ArrayList LoadFile(string path, string name)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e)
        {
            //Debug.Log("û���ļ�" + e.Message);
            return null;
        }
        string line;
        ArrayList arrList = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            arrList.Add(line);
        }
        sr.Close();
        sr.Dispose();
        return arrList;
    }

    [Obsolete]
    IEnumerator DownloadVoice()
    {
        WWW w = new WWW(get_url);
        yield return w;

        //��������Դ��ֵΪ�ⲿ���ص���������//

        myclip = w.GetAudioClip();
        AS.clip = myclip;
        //Debug.Log("����" + myclip.loadState + myclip.name);

        AS.Play();
        playTime = 0;
        //Debug.Log("url+++" + get_url);
    }

    private void FixedUpdate()
    {
        if (AS.clip == null) return;
        if (!isPause)
            playTime += Time.fixedDeltaTime;
        if (playTime > AS.clip.length && !AS.loop)
        {
            afterBtn();
        }
        else if(playTime > AS.clip.length && AS.loop)
        {
            playTime = 0;
            isPause = false;
        }

        timeSlider.maxValue = AS.clip.length;
        timeSlider.value = (float)playTime;

        hour = Convert.ToInt16(((int)playTime % 86400) / 3600);
        minute = Convert.ToInt16(((int)playTime % 86400 % 3600) / 60);
        second = Convert.ToInt16((int)playTime % 86400 % 3600 % 60);


        Ahour = Convert.ToInt16(((int)AS.clip.length % 86400) / 3600);
        Aminute = Convert.ToInt16(((int)AS.clip.length % 86400 % 3600) / 60);
        Asecond = Convert.ToInt16((int)AS.clip.length % 86400 % 3600 % 60);


        nowTime.text = hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2");
        allTime.text = Ahour.ToString("D2") + ":" + Aminute.ToString("D2") + ":" + Asecond.ToString("D2");
    }
}