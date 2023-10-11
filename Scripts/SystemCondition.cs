using UnityEngine;
using System.Runtime.InteropServices;
using System;
using TMPro;
using UnityEngine.Profiling;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

public class SystemCondition : MonoBehaviour
{
    // -----------CPU---------------
    private float updateInterval = 1;
    private int processorCount; // The amount of physical CPU cores
    private float CpuUsage; // output

    private Thread _cpuThread;
    private float _lasCpuUsage;

    private TextMeshProUGUI cpu1;


    // ------------Memory-------------
    #region Memory
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        //ϵͳ�ڴ�����
        public ulong ullTotalPhys;
        //ϵͳ�����ڴ�
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    //extern  ���� ����ƽ̨ dll �ؼ���
    [DllImport("kernel32.dll")]
    protected static extern void GlobalMemoryStatus(ref MEMORYSTATUSEX lpBuff);


    /// <summary>
    /// ��ȡ�����ڴ�
    /// PC
    /// </summary>
    /// <returns></returns>
    protected ulong GetWinAvailMemory()
    {
        MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
        ms.dwLength = 64;
        GlobalMemoryStatus(ref ms);
        return ms.ullAvailPhys;
    }

    /// <summary>
    /// ��ȡ���ڴ�
    /// PC
    /// </summary>
    /// <returns></returns>
    protected ulong GetWinTotalMemory()
    {
        MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
        ms.dwLength = 64;
        GlobalMemoryStatus(ref ms);
        return ms.ullTotalPhys;
    }

    /// <summary>
    /// ����Ӧ��ʹ�õ��ڴ�
    /// PC
    /// </summary>
    /// <returns></returns>
    protected long GetWinUsedMemory()
    {
        return Profiler.GetTotalReservedMemoryLong();
    }

    #endregion
    private TextMeshProUGUI memory1;
    private TextMeshProUGUI memory2;

    private double AllMemory;
    private double UsedMemory;

    // ------------Network--------------


    // -----------GraphicsDevice-------------
    private TextMeshProUGUI GraphicsDeviceName;
    private TextMeshProUGUI GraphicsDeviceVendorAndId;
    private TextMeshProUGUI GraphicsDeviceVersion;



    private void Awake()
    {
        #region component
        cpu1 = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        memory1 = transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        memory2 = transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
        #endregion


        #region CPU
        CpuUsage = 0;
        // setup the thread
        _cpuThread = new Thread(UpdateCPUUsage)
        {
            IsBackground = true,
            // we don't want that our measurement thread
            // steals performance
            Priority = System.Threading.ThreadPriority.BelowNormal
        };

        // start the cpu usage thread
        _cpuThread.Start();
        #endregion

        #region GraphicsDevice
        GraphicsDeviceName = transform.GetChild(1).GetChild(6).GetComponent<TextMeshProUGUI>();
        GraphicsDeviceVendorAndId = transform.GetChild(1).GetChild(7).GetComponent<TextMeshProUGUI>();
        GraphicsDeviceVersion = transform.GetChild(1).GetChild(8).GetComponent<TextMeshProUGUI>();

        GraphicsDeviceName.text = "显卡 : " + SystemInfo.graphicsDeviceName;
        GraphicsDeviceVendorAndId.text = "供应商ID :" + SystemInfo.graphicsDeviceVendorID.ToString();
        GraphicsDeviceVersion.text = "版本 : " + SystemInfo.graphicsDeviceVersion;
        #endregion
    }
    void Update()
    {
        // =======================CPU=======================
#if !UNITY_EDITOR
            processorCount = SystemInfo.processorCount / 2;
#endif
        // for more efficiency skip if nothing has changed
        if (Mathf.Approximately(_lasCpuUsage, CpuUsage)) return;

        // the first two values will always be "wrong"
        // until _lastCpuTime is initialized correctly
        // so simply ignore values that are out of the possible range
        if (CpuUsage < 0 || CpuUsage > 100) return;

        // I used a float instead of int for the % so use the ToString you like for displaying it

        cpu1.text = "CPU : " + CpuUsage.ToString("F1") + "%";
        
        // Update the value of _lasCpuUsage
        _lasCpuUsage = CpuUsage;


        // =======================Memory=======================
        AllMemory = GetWinTotalMemory() / 1024.0 / 1024 / 1024;
        UsedMemory = AllMemory - GetWinAvailMemory() / 1024.0 / 1024 / 1024;
        memory1.text = "Memory : " + (UsedMemory / (float)AllMemory * 100).ToString("#.#") + "%";
        memory2.text = UsedMemory.ToString("#.#") + " / " + AllMemory.ToString("#.#") + " G";

    }

    private void UpdateCPUUsage()
    {
        var lastCpuTime = new TimeSpan(0);

        // This is ok since this is executed in a background thread
        while (true)
        {
            var cpuTime = new TimeSpan(0);

            // Get a list of all running processes in this PC
            var AllProcesses = Process.GetProcesses();
            // Sum up the total processor time of all running processes
            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            // get the difference between the total sum of processor times
            // and the last time we called this
            var newCPUTime = cpuTime - lastCpuTime;

            // update the value of _lastCpuTime
            lastCpuTime = cpuTime;

            // The value we look for is the difference, so the processor time all processes together used
            // since the last time we called this divided by the time we waited
            // Then since the performance was optionally spread equally over all physical CPUs
            // we also divide by the physical CPU count

            
            CpuUsage = 100f * (float)newCPUTime.TotalSeconds / updateInterval / processorCount;

            // Wait for UpdateInterval
            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // We want only the physical cores but usually
        // this returns the twice as many virtual core count
        //
        // if this returns a wrong value for you comment this method out
        // and set the value manually
        processorCount = SystemInfo.processorCount / 2;

    }
#endif
    private void OnDisable()
    {
        _cpuThread?.Abort();
        Destroy(GetComponent<SystemCondition>());
    }
}
