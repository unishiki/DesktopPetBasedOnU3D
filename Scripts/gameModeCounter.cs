using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameModeCounter : MonoBehaviour
{
    #region
    //安装钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


    //卸载钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    //向下传递钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    //获取程序集模块的句柄
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    //全局钩子键盘为13
    private const int WH_KEYBOARD_LL = 13;

    //按下键
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;


    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYUP = 0x0105;

    private static LowLevelKeyboardProc _proc = HookCallback;

    private static IntPtr _hookID = IntPtr.Zero;
    private static Animator anim;
    private static bool[] animBool = new bool[4] {false,false,false,false};
    #endregion
    void Start()
    {
        _hookID = SetHook(_proc);
        if (anim == null)
            anim = UIScript.Instance.modleInGameMode.GetComponent<Animator>();
    }
    void OnApplicationQuit()
    {
        UnhookWindowsHookEx(_hookID);
    }
    private void OnDisable() // 必须卸载钩子，否则闪退
    {
        UnhookWindowsHookEx(_hookID);
    }

    //安装Hook,用于截获键盘。
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


    // kps 计算
    public static float timeFirst = 0;
    public static float timeNext = 0;
    public static float kps;
    public static int cnt = 0;
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            int vkCode = Marshal.ReadInt32(lParam);
            for (int i = 0; i < UIScript.Instance.keyObjs.Count; i++)
            {
                if (UIScript.Instance.gameModeKeys[i] == vkCode)
                {
                    UIScript.Instance.keyObjs[i].GetComponent<RawImage>().color = new Vector4(255, 255, 255, 255);

                    if (UIScript.Instance.gameModeKeys.Count == 5)
                    {
                        animBool[i] = true;
                        UIScript.Instance.keyObjs[i].GetComponent<RawImage>().material = UIScript.Instance.inGame4KMat;
                    }
                    if (animBool[0] || animBool[1])
                    {
                        anim.SetBool("R", true);
                    }
                    if (animBool[2] || animBool[3])
                    {
                        anim.SetBool("L", true);
                    }
                }
            }
        }

        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
        {
            // kps 计算
            timeNext = Time.realtimeSinceStartup;
            if (timeNext - timeFirst < .8f) // 1
            {
                cnt++;
            }
            else
            {
                kps = cnt / (timeNext - timeFirst) + 1.5f;
                UIScript.Instance.keyObjs[UIScript.Instance.keyObjs.Count - 1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = kps.ToString("F3");
                timeFirst = timeNext;
                cnt = 0;
            }


            int vkCode = Marshal.ReadInt32(lParam);

            for (int i = 0; i < UIScript.Instance.keyObjs.Count; i++)
            {
                if (UIScript.Instance.gameModeKeys[i] == vkCode)
                {
                    if (UIScript.Instance.gameModeKeys.Count == 5)
                    {
                        animBool[i] = false;
                        if (UIScript.Instance.keyObjs[i].GetComponent<RawImage>().material != null)
                        {
                            UIScript.Instance.keyObjs[i].GetComponent<RawImage>().material = null;
                        }
                    }
                    if (!animBool[0] && !animBool[1])
                    {
                        anim.SetBool("R", false);
                    }
                    if (!animBool[2] && !animBool[3])
                    {
                        anim.SetBool("L", false);
                    }

                    UIScript.Instance.keyObjs[i].GetComponent<RawImage>().color = new Vector4(0, 251, 255, 255);

                    UIScript.Instance.keyObjsNum[i] += 1;
                    UIScript.Instance.keyObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = UIScript.Instance.keyObjsNum[i].ToString();

                    if (UIScript.Instance.keyObjsNum[i] == 1000000)
                    {
                        UIScript.Instance.keyObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 10;
                    }
                    else if (UIScript.Instance.keyObjsNum[i] == 100000000)
                    {
                        UIScript.Instance.keyObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 9;
                    }
                    else if (UIScript.Instance.keyObjsNum[i] == 1000000000)
                    {
                        UIScript.Instance.keyObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 8;
                    }
                    else if (UIScript.Instance.keyObjsNum[i] >= int.MaxValue || UIScript.Instance.keyObjsNum[i] < 0)
                    {
                        UIScript.Instance.keyObjsNum[i] = 0;
                        UIScript.Instance.keyObjs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 14;
                    }
                }
            }

        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);//传给下一个钩子
    }

}
