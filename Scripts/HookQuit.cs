using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class HookQuit : MonoBehaviour
{
    #region
    //��װ����
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


    //ж�ع���
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    //���´��ݹ���
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    //��ȡ����ģ��ľ��
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    //ȫ�ֹ��Ӽ���Ϊ13
    private const int WH_KEYBOARD_LL = 13;

    //���¼�
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;

    private static LowLevelKeyboardProc _proc = HookCallback;

    private static IntPtr _hookID = IntPtr.Zero;
    #endregion
    void Start()
    {
        _hookID = SetHook(_proc);
    }

    void OnApplicationQuit()
    {
        UnhookWindowsHookEx(_hookID);
    }

    //��װHook,���ڽػ���̡�
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        //nCode>0��ʾ����Ϣ����Hook����������,���ύ��Windows���ڹ��̴�����
        //nCode=0���ʾ��Ϣ�������ݸ�Window��Ϣ������
        if (nCode >= 0 && wParam == (IntPtr)WM_SYSKEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            //UnityEngine.Debug.Log("Keydown:" + vkCode);

            if (vkCode == 113) // Alt + F2
            {
                //GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("exit", true);
                //if(!GameObject.FindWithTag("Player").GetComponent<Animator>().GetBool("exit"))
                //    Application.Quit();
                UIScript.Instance.quitSoft();
            }
        }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);//������һ������
    }

}
