using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class KeepKeyboard : MonoBehaviour
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

    private static LowLevelKeyboardProc _proc = HookCallback;

    private static IntPtr _hookID = IntPtr.Zero;
    #endregion
    public static int[] sendKeys; 
    void Start()
    {
        _hookID = SetHook(_proc);
        sendKeys = new int[15]; // 设定:最大记录13个
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

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        //nCode>0表示此消息已由Hook函数处理了,不会交给Windows窗口过程处理了
        //nCode=0则表示消息继续传递给Window消息处理函数
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            int vkCode = Marshal.ReadInt32(lParam);

            // 后续增加vkcode可取范围 if(...){}
            for (int i = 0; i < sendKeys.Length; i++) 
            {
                // 遍历去掉重复
                if (sendKeys[i] == vkCode)
                {
                    break;
                }
                // 增加
                if (sendKeys[i] == 0)
                {
                    sendKeys[i] = vkCode;
                    //UnityEngine.Debug.Log(vkCode);
                    break;
                }
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);//传给下一个钩子
    }
}
