using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
public class UIScript : MonoBehaviour
{
    string productName;
    private IntPtr hwnd;

    //private float timer;
    
    private struct RECT
    {
        public int Left; //最左坐标
        public int Top; //最上坐标
        public int Right; //最右坐标
        public int Bottom; //最下坐标
    }

    private const int SWP_SHOWWINDOW = 0x0040; //显示窗口 

    #region Win
    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    [DllImport("user32.dll")]
    static extern int GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    [DllImport("user32.dll")]
    public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
    // shell Opacity test
    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
    private static extern uint SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

    public static extern int GetWindowText(IntPtr hWnd, string lpString, int nMaxCount);
    // 获得桌面窗口句柄
    [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr GetDesktopWindow();

    /// <summary>
    /// 该函数返回与指定窗口有特定关系（如Z序或所有者）的窗口句柄。
    /// 函数原型：HWND GetWindow（HWND hWnd，UNIT nCmd）；
    /// </summary>
    /// <param name="hWnd">窗口句柄。要获得的窗口句柄是依据nCmd参数值相对于这个窗口的句柄。
    /// <param name="uCmd">说明指定窗口与要获得句柄的窗口之间的关系。该参数值参考GetWindowCmd枚举。
    /// <returns>返回值：如果函数成功，返回值为窗口句柄；如果与指定窗口有特定关系的窗口不存在，则返回值为NULL。
    /// 若想获得更多错误信息，请调用GetLastError函数。
    /// 备注：在循环体中调用函数EnumChildWindow比调用GetWindow函数可靠。调用GetWindow函数实现该任务的应用程序可能会陷入死循环或退回一个已被销毁的窗口句柄。
    /// 速查：Windows NT：3.1以上版本；Windows：95以上版本；Windows CE：1.0以上版本；头文件：winuser.h；库文件：user32.lib。
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

    /// <summary>
    /// 窗口与要获得句柄的窗口之间的关系。
    /// </summary>
    enum GetWindowCmd : uint
    {
        /// <summary>
        /// 返回的句柄标识了在Z序最高端的相同类型的窗口。
        /// 如果指定窗口是最高端窗口，则该句柄标识了在Z序最高端的最高端窗口；
        /// 如果指定窗口是顶层窗口，则该句柄标识了在z序最高端的顶层窗口：
        /// 如果指定窗口是子窗口，则句柄标识了在Z序最高端的同属窗口。
        /// </summary>
        GW_HWNDFIRST = 0,
        /// <summary>
        /// 返回的句柄标识了在z序最低端的相同类型的窗口。
        /// 如果指定窗口是最高端窗口，则该柄标识了在z序最低端的最高端窗口：
        /// 如果指定窗口是顶层窗口，则该句柄标识了在z序最低端的顶层窗口；
        /// 如果指定窗口是子窗口，则句柄标识了在Z序最低端的同属窗口。
        /// </summary>
        GW_HWNDLAST = 1,
        /// <summary>
        /// 返回的句柄标识了在Z序中指定窗口下的相同类型的窗口。
        /// 如果指定窗口是最高端窗口，则该句柄标识了在指定窗口下的最高端窗口：
        /// 如果指定窗口是顶层窗口，则该句柄标识了在指定窗口下的顶层窗口；
        /// 如果指定窗口是子窗口，则句柄标识了在指定窗口下的同属窗口。
        /// </summary>
        GW_HWNDNEXT = 2,
        /// <summary>
        /// 返回的句柄标识了在Z序中指定窗口上的相同类型的窗口。
        /// 如果指定窗口是最高端窗口，则该句柄标识了在指定窗口上的最高端窗口；
        /// 如果指定窗口是顶层窗口，则该句柄标识了在指定窗口上的顶层窗口；
        /// 如果指定窗口是子窗口，则句柄标识了在指定窗口上的同属窗口。
        /// </summary>
        GW_HWNDPREV = 3,
        /// <summary>
        /// 返回的句柄标识了指定窗口的所有者窗口（如果存在）。
        /// GW_OWNER与GW_CHILD不是相对的参数，没有父窗口的含义，如果想得到父窗口请使用GetParent()。
        /// 例如：例如有时对话框的控件的GW_OWNER，是不存在的。
        /// </summary>
        GW_OWNER = 4,
        /// <summary>
        /// 如果指定窗口是父窗口，则获得的是在Tab序顶端的子窗口的句柄，否则为NULL。
        /// 函数仅检查指定父窗口的子窗口，不检查继承窗口。
        /// </summary>
        GW_CHILD = 5,
        /// <summary>
        /// （WindowsNT 5.0）返回的句柄标识了属于指定窗口的处于使能状态弹出式窗口（检索使用第一个由GW_HWNDNEXT 查找到的满足前述条件的窗口）；
        /// 如果无使能窗口，则获得的句柄与指定窗口相同。
        /// </summary>
        GW_ENABLEDPOPUP = 6
    }
    /*GetWindowCmd指定结果窗口与源窗口的关系，它们建立在下述常数基础上：
       GW_CHILD
       寻找源窗口的第一个子窗口
       GW_HWNDFIRST
       为一个源子窗口寻找第一个兄弟（同级）窗口，或寻找第一个顶级窗口
       GW_HWNDLAST
       为一个源子窗口寻找最后一个兄弟（同级）窗口，或寻找最后一个顶级窗口
       GW_HWNDNEXT
       为源窗口寻找下一个兄弟窗口
       GW_HWNDPREV
       为源窗口寻找前一个兄弟窗口
       GW_OWNER
       寻找窗口的所有者
    */


    

    #endregion

    [SerializeField] private GameObject modle;
    [SerializeField] public GameObject modleInGameMode;
    private Animator anim;
    // Btn1 RandomAnim
    public bool randomAnimMode;
    [SerializeField] private GameObject btn1Sign;
    [SerializeField] private RandomAnimModeCameraPosition rdmAnimModeScript;
    private float oriSize;
    private Vector3 timeObjOrgPos;
    [SerializeField] private GameObject mainPagePS;
    // Btn2 Size
    [SerializeField] private Slider sizeSlider;
    [SerializeField] public GameObject sizePage;
    private Vector3 oriMainPagePSSize = new Vector3(.2f, .2f, .2f);
    private float tempSize = 0;
    private float sizeNum;
    [SerializeField] private TextMeshProUGUI sizeNumTmp;
    // Btn3 SystemTime
    [SerializeField] private GameObject timeObj;
    [SerializeField] private GameObject btn3Sign;
    // Btn4 AI
    [SerializeField] private GameObject AIPannel;
    [SerializeField] private GameObject btn4Sign;
    public bool inAIMode;
    // Btn5 workMode
    [SerializeField] private GameObject btn5Sign;
    [SerializeField] private GameObject workModePlace;
    public bool inWorkMode;
    // Btn6 GameMode
    [SerializeField] private GameObject btn6Sign;
    public List<int> gameModeKeys;
    public List<int> keyObjsNum;
    public List<GameObject> keyObjs;
    [SerializeField] private GameObject keysPlace;
    [SerializeField] private GameObject keyProfab;
    [SerializeField] private GameObject gameModeContinuePage;
    [SerializeField] private GameObject mainMenu;
    public bool inGameMode;
    public Material inGame4KMat;
    // Btn7 HideShell
    private bool shellIsHide;
    IntPtr trayHwnd;
    [SerializeField] private GameObject btn7Sign;
    // Btn8 SetOpacity
    [SerializeField] private Slider opacitySlider;
    [SerializeField] public GameObject opacityPage;
    [SerializeField] private TextMeshProUGUI opacityNumTmp;
    // Btn9 OnTop
    [SerializeField] private GameObject btn8Sign;
    // Btn10、11 AllTransparent
    [SerializeField] private GameObject btn10Sign;
    [SerializeField] private GameObject btn11Sign;
    private IntPtr desktopPtr;// 桌面窗口句柄
    private List<IntPtr> allOtherHwnds = new List<IntPtr>();
    private List<int> allOtherHdStyle = new List<int>(); // 所有窗口样式
    private string otherWinTitle = "";
    private int allWindowsOpacityValue;
    [SerializeField] private Slider allWindowsOpacitySlider;
    [SerializeField] public GameObject setAllWindowsOpacitySliderPage;
    [SerializeField] private TextMeshProUGUI allOpacityNumTmp;
    // Btn12 SimpleMode
    [SerializeField] private GameObject btn12Sign;
    [SerializeField] private GameObject simpleModeGameObj;
    public bool inSimpleMode;
    // Btn13 SystemCondition
    [SerializeField] private GameObject btn13;
    [SerializeField] private GameObject systemConditionPage;
    // Btn14 AutoMemoryClear
    [SerializeField] private GameObject btn14Sign;
    // Btn15 ListeningMode
    [SerializeField] private GameObject ListeningPannel;
    [SerializeField] private GameObject btn15Sign;
    private bool inListeningMode;
    // Btn16 CountAndDeleteRepeatPage
    [SerializeField] private GameObject CountAndDeleteRepeatPanel;
    [SerializeField] private GameObject btn16Sign;
    private bool inDelReMode;


    private int intExTemp;
    private int oriEx;
    private bool swap;

    [Header("按钮")]
    [SerializeField] public List<GameObject> btns;
    const float firstY = 350;

    public static UIScript Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
        anim = modle.GetComponent<Animator>();
        // Win
        productName = Application.productName;
        // Btn1 RandomAnim
        randomAnimMode = false;
        oriSize = 60;
        timeObjOrgPos = new Vector3(106f, 500f, 0);
        // Btn2 SizePage
        sizeNumTmp.text = "50%";
        // Btn3 SystemTime
        timeObj.transform.localPosition = new Vector3(106f, 500f, 0);
        // Btn4 AiMode
        inAIMode = false;
        // Btn5 workMode
        inWorkMode = false;
        // Btn6 GameMode
        gameModeKeys = new List<int>();
        keyObjs = new List<GameObject>();
        inGameMode = false;
        // Btn7 HideShell
        shellIsHide = false;
        trayHwnd = FindWindow("Shell_TrayWnd", null); // 任务栏Shell_TrayWnd
        // 任务栏透明度设置
        intExTemp = GetWindowLong(trayHwnd, GWL_EXSTYLE); // 获得当前样式
        oriEx = intExTemp;
        SetWindowLong(trayHwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED); // 当前样式加上WS_EX_LAYERED     // WS_EX_TRANSPARENT 收不到点击的透明
        // 隐藏/显示任务栏
        ShowWindow(trayHwnd, 1);

        // Btn9 SetOpacity
        swap = true;
        // Btn 10 
        allWindowsOpacityValue = 170;
        allWindowsOpacitySlider.value = allWindowsOpacityValue;
        allOpacityNumTmp.text = allWindowsOpacityValue.ToString();

        // Btn12
        inSimpleMode = false;

        // Btn15
        inListeningMode = false;
        // Btn16
        inDelReMode = false;
    }

    private void OnDestroy() // 退出程序时还原用户原设置样式
    {
        ShowWindow(trayHwnd, 1);
        SetWindowLong(trayHwnd, GWL_EXSTYLE, oriEx);
        if (btn10Sign.activeSelf)
        {
            for (int i = 0; i < allOtherHwnds.Count; i++)
            {
                SetLayeredWindowAttributes(allOtherHwnds[i], 0, 255, 2);
            }

            for (int i = 0; i < allOtherHdStyle.Count; i++)
            {
                SetWindowLong(allOtherHwnds[i], GWL_EXSTYLE, allOtherHdStyle[i]);
            }
        }
    }
    // Btn1
    public void randomAnim()
    {
        setSizePageActiveFalse();
        if (inWorkMode || inGameMode || inAIMode || gameModeContinuePage.activeSelf || inSimpleMode || inListeningMode || inDelReMode) return;


        if (!randomAnimMode)
        {
            oriSize = Camera.main.fieldOfView;
            Camera.main.fieldOfView = 60f;
            Camera.main.farClipPlane = 10f;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 3.7f);
            oriMainPagePSSize = mainPagePS.transform.localScale;
            mainPagePS.transform.localScale = new Vector3(.068f, .068f, .068f);
            

            timeObjOrgPos = timeObj.transform.localPosition;
            timeObj.transform.localPosition = new Vector3(106f, 500f, 0);

            rdmAnimModeScript.enabled = true;
            randomAnimMode = true;
            anim.SetBool("randomAnim",true);
            
            mainMenu.SetActive(false);
            btn1Sign.SetActive(true);

            
        }
        else if (randomAnimMode)
        {
            Camera.main.fieldOfView = oriSize;
            Camera.main.transform.position = new Vector3(0, .8f, 1.5f);
            Camera.main.farClipPlane = 1.5f;
            mainPagePS.transform.localScale = oriMainPagePSSize;

            timeObj.transform.localPosition = timeObjOrgPos;
            rdmAnimModeScript.enabled = false;
            randomAnimMode = false;
            anim.SetBool("randomAnim", false);
            mainMenu.SetActive(false);
            btn1Sign.SetActive(false);
            

            modle.SetActive(false);
            modle.transform.position = new Vector3(0, .2f, -.4f);
            modle.transform.rotation = Quaternion.Euler(0, .2f, -.4f);
            modle.SetActive(true);
        }
    }
    // Btn2_setSizePage_Slider
    public void setSize()
    {
        Camera.main.fieldOfView = sizeSlider.value;
        sizeNum = 100f - ((sizeSlider.value - 100) * 2.5f);
        if (sizeNum > 99.5f)
            sizeNumTmp.text = "100%";
        else if (sizeNum < .5f)
            sizeNumTmp.text = "0.1%";
        else
            sizeNumTmp.text = sizeNum < 10 ? sizeNum.ToString("##") + "%" : sizeNum.ToString("##") + "%";

        timeObj.transform.localPosition = new Vector3(106f, 1460f - sizeSlider.value * 8, 0);

        if (sizeSlider.value < 120)
        {
            tempSize = .2f - .065f * (120 - sizeSlider.value) / 20f;
        }
        else if (sizeSlider.value == 120)
        {
            tempSize = .2f;
        }
        else if (sizeSlider.value > 120 && sizeSlider.value < 130)
        {
            tempSize = .2f + .04f * (sizeSlider.value - 120) / 10f;
        }
        else if (sizeSlider.value >= 130)
        {
            tempSize = .24f + .07f * (sizeSlider.value - 130) / 10f;
        }
        mainPagePS.transform.localScale = new Vector3(tempSize, tempSize, tempSize); // 100 - 140
    }
    // Btn2
    public void mainBtnSetSize()
    {
        opacityPage.SetActive(false);
        setAllWindowsOpacitySliderPage.SetActive(false);
        if (inGameMode || randomAnimMode || inWorkMode || inAIMode || gameModeContinuePage.activeSelf) return;
        if (MouseEvents.Instance.point.X <= Screen.currentResolution.width / 2)
        {
            sizePage.transform.localPosition = new Vector3(30.7f, -11.6f, 0);
        }
        else if (MouseEvents.Instance.point.X > Screen.currentResolution.width / 2)
        {
            sizePage.transform.localPosition = new Vector3(-30.7f, -11.6f, 0);
        }
        sizePage.SetActive(true);
    }
    // Btn3
    public void showTime()
    {
        setSizePageActiveFalse();
        timeObj.SetActive(!timeObj.activeSelf);
        btn3Sign.SetActive(timeObj.activeSelf);
    }
    // Btn4 (原)
    #region 内存回收
    [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
    public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
    /// <summary>
    /// 释放内存
    /// </summary>
    public static void ClearMemory()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
    }

    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hwProc);
    // 只在工作模式时存在Update调用、关闭菜单时调用一次、Btn14控制
    public void clearMemory()
    {
        ClearMemory();
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes)
        {
            try
            {
                EmptyWorkingSet(process.Handle);
            }
            catch
            {

            }
        }
    }
    #endregion
    // Btn4
    public void AiMode()
    {
        setSizePageActiveFalse();
        if (inWorkMode || inGameMode || randomAnimMode || gameModeContinuePage.activeSelf || inListeningMode || inDelReMode) return;
        if (!inAIMode)
        {
            inAIMode = true;
            AIPannel.SetActive(true);
            mainMenu.SetActive(false);
            btn4Sign.SetActive(true);
        }
        else if (inAIMode)
        {
            inAIMode = false;
            AIPannel.SetActive(false);
            mainMenu.SetActive(false);
            btn4Sign.SetActive(false);
        }
    }
    // Btn5
    public void workMode()
    {
        setSizePageActiveFalse();
        if (inGameMode || randomAnimMode || inAIMode || gameModeContinuePage.activeSelf || inListeningMode || inDelReMode) return;
        if (!inWorkMode)
        {
            if (inSimpleMode)
            {
                inSimpleMode = false;
                btn12Sign.SetActive(false);
                simpleModeGameObj.SetActive(false);
                modle.transform.position = new Vector3(0, .2f, -.4f);
                modle.transform.rotation = Quaternion.Euler(0, .2f, -.4f);
            }
            inWorkMode = true;
            btn5Sign.SetActive(true);

            modle.SetActive(false);
            workModePlace.SetActive(true);
            mainMenu.SetActive(false);
            // 进入工作模式自动开启一次自动清理内存
            if (!btn14Sign.activeSelf)
            {
                //gameObject.AddComponent<ClearMemory>();
                setAutoMemoryClear();
            }
            
        }
        else if (inWorkMode)
        {
            inWorkMode = false;
            btn5Sign.SetActive(false);

            modle.transform.position = new Vector3(0, .2f, -.4f);
            modle.transform.rotation = Quaternion.Euler(0, .2f, -.4f);
            modle.SetActive(true);
            workModePlace.SetActive(false);
            mainMenu.SetActive(false);
            // 退出工作模式自动关闭自动清理内存
            if (btn14Sign.activeSelf)
            {
                setAutoMemoryClear();
            }
            

            Camera.main.gameObject.GetComponent<AudioSource>().Stop();
        }
    }

    // Btn6
    public void gameMode()
    {
        setSizePageActiveFalse();
        if (randomAnimMode || inWorkMode || inAIMode || inListeningMode || inDelReMode) return;
        if (inGameMode) // 退出音游模式
        {
            inGameMode = false;
            modleInGameMode.SetActive(false);
            keysPlace.transform.localPosition = new Vector3(0, 0, 0);
            keysPlace.transform.localRotation = Quaternion.Euler(0, 0, 0);
            keysPlace.transform.localScale = new Vector3(1, 1, 1);
            modle.transform.position = new Vector3(0, .2f, -.4f);
            modle.transform.rotation = Quaternion.Euler(0, .2f, -.4f);
            modle.SetActive(true);
            btn6Sign.SetActive(false);
            Destroy(keysPlace.GetComponent<gameModeCounter>());
            // 销毁keys
            for(int i = 0; i < keyObjs.Count; i++)
            {
                Destroy(keysPlace.transform.GetChild(i).GetComponent<RawImage>());
                keysPlace.transform.GetChild(i).GetComponent<keyDestorySelf>().destroySelf();
            }
            mainMenu.SetActive(false);
        }
        else if (!inGameMode)
        {
            gameModeContinuePage.AddComponent<KeepKeyboard>();
            gameModeContinuePage.transform.localPosition = new Vector3(0, 800f, 0);
            gameModeContinuePage.SetActive(true);
            mainMenu.SetActive(false);
        }

    }
    // Btn6-1
    public void determineKeys()
    {
        // 获取和判断上个界面的用户输入
        Destroy(gameModeContinuePage.GetComponent<KeepKeyboard>()); // 只能通过卸载脚本的方式，否则数组只能获取一次，或钩子问题闪退
        gameModeKeys.Clear();
        keyObjs.Clear();
        keyObjsNum.Clear();
        var tmpKeys = KeepKeyboard.sendKeys;
        for (int i = 0; i < tmpKeys.Length; i++)
        {
            if (tmpKeys[i] != 0)
                gameModeKeys.Add(tmpKeys[i]);
        }
        gameModeKeys.Add(999); // kps key
        if (gameModeKeys.Count == 0 || gameModeKeys.Count > 13)
        {
            return;
        }

        // 真正进入音游模式
        if (inSimpleMode)
        {
            //setSimpleMode(); // 退出简易模式
            inSimpleMode = false;
            btn12Sign.SetActive(false);
            simpleModeGameObj.SetActive(false);
            modle.transform.position = new Vector3(0, .2f, -.4f);
            modle.transform.rotation = Quaternion.Euler(0, .2f, -.4f);
        }
        modle.SetActive(false);
        btn6Sign.SetActive(true);
        inGameMode = true;
        
        for (int i = 0; i < gameModeKeys.Count; i++)
        {
            GameObject keys_tmps = Instantiate(keyProfab, keysPlace.transform);
            keyObjs.Add(keys_tmps);
            keyObjsNum.Add(0);
            keys_tmps.AddComponent<keyDestorySelf>();

            // 每个key格子位置
            if ((i + 1) <= gameModeKeys.Count / 2)
                keys_tmps.transform.localPosition = new Vector3((gameModeKeys.Count / 2 - i) * -256, 0, 0);
            else if ((i + 1) > gameModeKeys.Count / 2)
                keys_tmps.transform.localPosition = new Vector3((i - gameModeKeys.Count / 2) * 256, 0, 0);

            if(gameModeKeys.Count % 2 == 0)
            {
                keys_tmps.transform.localPosition += new Vector3(128f, 0, 0);
            }

            keys_tmps.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = getKeyText(gameModeKeys[i]);
            if (keys_tmps.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Length >= 4)
            {
                keys_tmps.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 16;
            }
            else if (keys_tmps.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Length >= 2)
            {
                keys_tmps.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 22;
            }
            
            keys_tmps.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = keyObjsNum[i].ToString();

        }

        if (gameModeKeys.Count == 5)
        {
            modleInGameMode.SetActive(true);
            keysPlace.transform.localPosition = new Vector3(-145.5f, -161f, 828f);
            keysPlace.transform.localRotation = Quaternion.Euler(60.5f, 25f, 180.5f);
            keysPlace.transform.localScale = new Vector3(.56f, .68f, 1f);
        }

        keysPlace.AddComponent<gameModeCounter>();
        
    }
    // Btn7
    public void hideShell()
    {
        setSizePageActiveFalse();
        if (!shellIsHide)
        {
            btn7Sign.SetActive(true);
            ShowWindow(trayHwnd, 0);
            shellIsHide = true;
        }
        else if (shellIsHide)
        {
            btn7Sign.SetActive(false);
            ShowWindow(trayHwnd, 1);//隐藏任务栏 0 1
            shellIsHide = false;
        }
    }
    // Btn8
    public void showOnTop()
    {
        setSizePageActiveFalse();
#if !UNITY_EDITOR
        if (btn8Sign.activeSelf)
        {
            btn8Sign.SetActive(false);

            hwnd = FindWindow(null, productName); 
            RECT fx = new RECT();
            GetWindowRect(hwnd, ref fx);
            SetWindowPos(hwnd, -2, fx.Left, fx.Top, 1200, 900, SWP_SHOWWINDOW);
        }
        else if (!btn8Sign.activeSelf)
        {
            btn8Sign.SetActive(true);

            hwnd = FindWindow(null, productName); 
            RECT fx = new RECT();
            GetWindowRect(hwnd, ref fx);
            SetWindowPos(hwnd, -1, fx.Left, fx.Top, 1200, 900, SWP_SHOWWINDOW);
        }
#endif

    }
    // Btn9 opacity shell
    //设定一个新的扩展风格
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x00080000;

    public void setShellOpacityBtn()
    {
        sizePage.SetActive(false);
        setAllWindowsOpacitySliderPage.SetActive(false);
        if (inWorkMode || inGameMode) return;
        if (MouseEvents.Instance.point.X <= Screen.currentResolution.width / 2)
        {
            opacityPage.transform.localPosition = new Vector3(30.7f, -11.6f, 0);
        }
        else if (MouseEvents.Instance.point.X > Screen.currentResolution.width / 2)
        {
            opacityPage.transform.localPosition = new Vector3(-30.7f, -11.6f, 0);
        }
        opacityPage.SetActive(true);
        
    }
    public void setShellOpacity()
    {
        if (opacitySlider.value > 254 && !swap)
        {
            swap = true;
            SetWindowLong(trayHwnd, GWL_EXSTYLE, intExTemp & ~WS_EX_LAYERED);
        }
        else if(opacitySlider.value <= 254 && swap)
        {
            swap = false;
            SetWindowLong(trayHwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED);
        }
        if (opacitySlider.value <= 254)
        {
            SetLayeredWindowAttributes(trayHwnd, 0, (int)opacitySlider.value, 2);
        }
        

        opacityNumTmp.text = ((int)opacitySlider.value).ToString();
    }

    // Btn10 AllTransparent
    public void allTransparent()
    {
        setSizePageActiveFalse();
        allOtherHwnds.Clear();
        allOtherHdStyle.Clear();
        // 保存自身程序style
        //var productName = Application.productName;
        IntPtr thishwnd = FindWindow(null, productName);
        //int tmpMyStyle = GetWindowLong(thishwnd, GWL_EXSTYLE);
        string windowTitle = "";
        int myWinIn = GetWindowText(thishwnd, windowTitle, 128);

        // 获得桌面窗口句柄
        desktopPtr = GetDesktopWindow();
        // 获得一个子窗口（这通常是一个顶层窗口，当前活动的窗口）
        IntPtr winPtr = GetWindow(desktopPtr, GetWindowCmd.GW_CHILD);
        otherWinTitle = "";
        int otherWinIn = GetWindowText(winPtr, otherWinTitle, 128);
        if (otherWinIn != myWinIn)
        {
            allOtherHwnds.Add(winPtr);
            allOtherHdStyle.Add(GetWindowLong(winPtr, GWL_EXSTYLE));
        }

        // 循环取得桌面下的所有子窗口
        while (winPtr != IntPtr.Zero)
        {
            otherWinTitle = "";
            otherWinIn = GetWindowText(winPtr, otherWinTitle, 128);
            if (otherWinIn != myWinIn)
            {
                allOtherHwnds.Add(winPtr);
                allOtherHdStyle.Add(GetWindowLong(winPtr, GWL_EXSTYLE));
            }
            // 继续获取下一个子窗口
            winPtr = GetWindow(winPtr, GetWindowCmd.GW_HWNDNEXT);
        }

        if (!btn10Sign.activeSelf)
        {
            btn10Sign.SetActive(true);
            btn11Sign.SetActive(true);
            for (int i = 0; i < allOtherHdStyle.Count; i++)
            {
                SetWindowLong(allOtherHwnds[i], GWL_EXSTYLE, allOtherHdStyle[i] | WS_EX_LAYERED);
            }

            for (int i = 0; i < allOtherHwnds.Count; i++)
            {
                SetLayeredWindowAttributes(allOtherHwnds[i], 0, allWindowsOpacityValue, 2);
            }
            //SetWindowLong(thishwnd, GWL_EXSTYLE, tmpMyStyle);
            //SetLayeredWindowAttributes(thishwnd, 0, 255, 1);
        }
        else if (btn10Sign.activeSelf)
        {
            btn10Sign.SetActive(false);
            btn11Sign.SetActive(false);
            for (int i = 0; i < allOtherHwnds.Count; i++)
            {
                SetLayeredWindowAttributes(allOtherHwnds[i], 0, 255, 2);
            }

            for (int i = 0; i < allOtherHdStyle.Count; i++)
            {
                SetWindowLong(allOtherHwnds[i], GWL_EXSTYLE, allOtherHdStyle[i]);
            }

            //SetWindowLong(thishwnd, GWL_EXSTYLE, tmpMyStyle);
            //SetLayeredWindowAttributes(thishwnd, 0, 255, 1);
        }
    }

    // Btn11 AllTransparentValue
    public void setAllOpacityValue()
    {
        allWindowsOpacityValue = (int)allWindowsOpacitySlider.value;
        allOpacityNumTmp.text = allWindowsOpacityValue.ToString();
        if (btn11Sign.activeSelf)
        {
            for (int i = 0; i < allOtherHwnds.Count; i++)
            {
                SetLayeredWindowAttributes(allOtherHwnds[i], 0, allWindowsOpacityValue, 2);
            }
        }
    }
    public void openSetAllOpaValuePage()
    {
        opacityPage.SetActive(false);
        sizePage.SetActive(false);
        if (inWorkMode || inGameMode) return;

        if (MouseEvents.Instance.point.X <= Screen.currentResolution.width / 2)
        {
            setAllWindowsOpacitySliderPage.transform.localPosition = new Vector3(30.7f, -11.6f, 0);
        }
        else if (MouseEvents.Instance.point.X > Screen.currentResolution.width / 2)
        {
            setAllWindowsOpacitySliderPage.transform.localPosition = new Vector3(-30.7f, -11.6f, 0);
        }
        setAllWindowsOpacitySliderPage.SetActive(true);
    }

    // Btn12 SimpleMode
    public void setSimpleMode()
    {
        if (inWorkMode || inGameMode || inAIMode || gameModeContinuePage.activeSelf || randomAnimMode) return;
        if (!inSimpleMode)
        {
            inSimpleMode = true;
            btn12Sign.SetActive(true);
            modle.SetActive(false);
            simpleModeGameObj.SetActive(true);
            mainMenu.SetActive(false);
        }
        else if(inSimpleMode)
        {
            inSimpleMode = false;
            btn12Sign.SetActive(false);
            simpleModeGameObj.SetActive(false);
            modle.transform.position = new Vector3(0, .2f, -.4f);
            modle.transform.rotation = Quaternion.Euler(0, .2f, -.4f);
            modle.SetActive(true);
            mainMenu.SetActive(false);
        }
    }

    // Btn13 SystemCondition
    public void SystemCondition()
    {
        opacityPage.SetActive(false);
        sizePage.SetActive(false);
        setAllWindowsOpacitySliderPage.SetActive(false);

        if (inWorkMode || inGameMode) return;

        if (!systemConditionPage.activeSelf)
        {
            if (MouseEvents.Instance.point.X <= Screen.currentResolution.width / 2)
            {
                systemConditionPage.transform.localPosition = new Vector3(30.7f, -11.6f, 0);
            }
            else if (MouseEvents.Instance.point.X > Screen.currentResolution.width / 2)
            {
                systemConditionPage.transform.localPosition = new Vector3(-30.7f, -11.6f, 0);
            }
            if (btn13.GetComponent<SystemCondition>() == null)
                btn13.AddComponent<SystemCondition>();
            systemConditionPage.SetActive(true);
        }
        else if (systemConditionPage.activeSelf)
        {
            systemConditionPage.SetActive(false);
            if (btn13.GetComponent<SystemCondition>() != null)
                Destroy(btn13.GetComponent<SystemCondition>());
        }
    }

    //

    //public static string GetDeviceInfo()
    //{
    //    return "deviceModel:" + SystemInfo.deviceModel + "\n" +/*设备模型*/
    //            "deviceName:" + SystemInfo.deviceName + "\n" +/*设备名称*/
    //            "deviceUniqueIdentifier:" + SystemInfo.deviceUniqueIdentifier + "\n" +/*设备唯一标识符*/
    //            "copyTextureSupport:" + SystemInfo.copyTextureSupport.ToString() + "\n" +/*是否支持纹理复制*/
    //            "graphicsDeviceID:" + SystemInfo.graphicsDeviceID.ToString() + "\n" +/*显卡ID*/
    //            "graphicsDeviceName:" + SystemInfo.graphicsDeviceName + "\n" +/*显卡名称*/
    //            "graphicsDeviceType:" + SystemInfo.graphicsDeviceType.ToString() + "\n" +/*显卡类型*/
    //            "graphicsDeviceVendor:" + SystemInfo.graphicsDeviceVendor + "\n" +/*显卡供应商*/
    //            "graphicsDeviceVendorID:" + SystemInfo.graphicsDeviceVendorID.ToString() + "\n" +/*显卡供应商ID*/
    //            "graphicsDeviceVersion:" + SystemInfo.graphicsDeviceVersion + "\n" +/*显卡版本号*/
    //            "graphicsMemorySize:" + SystemInfo.graphicsMemorySize + "\n" +/*显存大小（单位：MB）*/
    //            "graphicsMultiThreaded:" + SystemInfo.graphicsMultiThreaded.ToString() + "\n" +/*显卡是否支持多线程渲染*/
    //            "supportedRenderTargetCount:" + SystemInfo.supportedRenderTargetCount.ToString() + "\n" +/*支持的渲染目标数量*/
    //            "systemMemorySize:" + SystemInfo.systemMemorySize.ToString() + "\n";/*系统内存大小(单位：MB):*/
    //       //
    //}
        // Btn14 AutoMemoryClear
    public void setAutoMemoryClear()
    {
        if (btn14Sign.activeSelf)
        {
            Destroy(GetComponent<ClearMemory>());
            btn14Sign.SetActive(false);
        }
        else if (!btn14Sign.activeSelf)
        {
            gameObject.AddComponent<ClearMemory>();
            btn14Sign.SetActive(true);
        }
    }

    // Btn15 Listening
    public void setListeningMode()
    {
        setSizePageActiveFalse();
        if (inWorkMode || inGameMode || randomAnimMode || gameModeContinuePage.activeSelf || inAIMode || inDelReMode) return;
        if (!inListeningMode)
        {
            inListeningMode = true;
            ListeningPannel.SetActive(true);
            mainMenu.SetActive(false);
            btn15Sign.SetActive(true);
        }
        else if (inListeningMode)
        {
            inListeningMode = false;
            ListeningPannel.SetActive(false);
            mainMenu.SetActive(false);
            btn15Sign.SetActive(false);
        }
    }
    // Btn16 CountAndDeleteRepeat
    public void setCountAndDeleteRepeatPage()
    {
        setSizePageActiveFalse();
        if (inWorkMode || inGameMode || randomAnimMode || gameModeContinuePage.activeSelf || inAIMode || inListeningMode) return;
        if (!inDelReMode)
        {
            inDelReMode = true;
            CountAndDeleteRepeatPanel.SetActive(true);
            mainMenu.SetActive(false);
            btn16Sign.SetActive(true);
        }
        else if (inDelReMode)
        {
            inDelReMode = false;
            CountAndDeleteRepeatPanel.SetActive(false);
            mainMenu.SetActive(false);
            btn16Sign.SetActive(false);
        }
    }


    // BtnLast
    public void quitSoft()
    {
        mainMenu.SetActive(false);

        if (inGameMode || randomAnimMode || inWorkMode || inAIMode || inSimpleMode || inListeningMode)
        {
            Application.Quit();
        }

        anim.SetBool("exit", true);
        Invoke("appQuit", 3f);
            
    }

    // Btn Down
    public void onEnableSetUIPos()
    {
        for (int i = 0; i < btns.Count - 2; i++)
        {
            btns[i].transform.localPosition = new Vector3(btns[i].transform.localPosition.x,350 - i * 88, btns[i].transform.localPosition.z);
            if (btns[i].transform.localPosition.y < -266)
            {
                btns[i].SetActive(false);
            }
            else
            {
                btns[i].SetActive(true);
            }
        }
        btns[btns.Count - 2].SetActive(true); // ▼
        btns[btns.Count - 1].SetActive(false); // ▲
    }
    public void goDown() // ▼ 点击功能
    {
        setSizePageActiveFalse(); // 其他所有小窗口关闭

        // 第一个按钮处于显示状态，▲处于隐藏状态时，点击后 ▲显示，第一个按钮隐藏
        if (btns[0].transform.localPosition.y == 350 && !btns[btns.Count - 1].activeSelf)
        {
            btns[0].SetActive(false);
            btns[btns.Count - 1].SetActive(true);
            return;
        }
        // 关闭按钮btns[btns.Count - 3]处于最后的位置（-354），表示往下到底了，同时关闭按钮处于隐藏状态，点击后，则隐藏▼按钮 ，显示关闭按钮
        if (btns[btns.Count - 3].transform.localPosition.y == -354 && !btns[btns.Count - 3].activeSelf && btns[btns.Count - 2].activeSelf)
        {
            btns[btns.Count - 2].SetActive(false);
            btns[btns.Count - 3].SetActive(true);
            return;
        }

        // ▼ 的主要功能，每个按钮往上移动一格距离
        for (int i = 0; i < btns.Count - 2; i++)
        {
            btns[i].transform.localPosition = new Vector3(btns[i].transform.localPosition.x, btns[i].transform.localPosition.y + 88, btns[i].transform.localPosition.z);
            if (btns[i].transform.localPosition.y >= -266)
            {
                btns[i].SetActive(true);
            }
            if (btns[i].transform.localPosition.y > 262)
            {
                btns[i].SetActive(false);
            }
            if (btns[btns.Count - 3].transform.localPosition.y == -354)
            {
                btns[btns.Count - 2].SetActive(false);
                btns[btns.Count - 3].SetActive(true);
            }
        }
    }

    public void goUp()
    {
        setSizePageActiveFalse();
        if (btns[btns.Count - 3].transform.localPosition.y == -354 && !btns[btns.Count - 2].activeSelf)
        {
            btns[btns.Count - 3].SetActive(false);
            btns[btns.Count - 2].SetActive(true);
            return;
        }
        if (btns[0].transform.localPosition.y == 350 && !btns[0].activeSelf && btns[btns.Count - 1].activeSelf)
        {
            btns[btns.Count - 1].SetActive(false);
            btns[0].SetActive(true);
            return;
        }
        for (int i = 0; i < btns.Count - 2; i++)
        {
            btns[i].transform.localPosition = new Vector3(btns[i].transform.localPosition.x, btns[i].transform.localPosition.y - 88, btns[i].transform.localPosition.z);
            if (btns[i].transform.localPosition.y <= 262)
            {
                btns[i].SetActive(true);
            }
            if (btns[i].transform.localPosition.y < -266)
            {
                btns[i].SetActive(false);
            }
            if (btns[0].transform.localPosition.y == 350)
            {
                btns[btns.Count - 1].SetActive(false);
                btns[0].SetActive(true);
            }
        }
    }

    private void appQuit()
    {
        Application.Quit();
    }

    private string getKeyText(int vcode)
    {
        string s = "null";
        switch (vcode)
        {
            case 65: s = "A";break;
            case 66: s = "B";break;
            case 67: s = "C";break;
            case 68: s = "D";break;
            case 69: s = "E";break;
            case 70: s = "F";break;
            case 71: s = "G";break;
            case 72: s = "H";break;
            case 73: s = "I";break;
            case 74: s = "J";break;
            case 75: s = "K";break;
            case 76: s = "L";break;
            case 77: s = "M";break;
            case 78: s = "N";break;
            case 79: s = "O";break;
            case 80: s = "P";break;
            case 81: s = "Q";break;
            case 82: s = "R";break;
            case 83: s = "S";break;
            case 84: s = "T";break;
            case 85: s = "U";break;
            case 86: s = "V";break;
            case 87: s = "W";break;
            case 88: s = "X";break;
            case 89: s = "Y";break;
            case 90: s = "Z";break;

            case 27: s = "Esc";break;
            case 112: s = "F1";break;
            case 113: s = "F2";break;
            case 114: s = "F3";break;
            case 115: s = "F4";break;
            case 116: s = "F5";break;
            case 117: s = "F6";break;
            case 118: s = "F7";break;
            case 119: s = "F8";break;
            case 120: s = "F9";break;
            case 121: s = "F10";break;
            case 122: s = "F11";break;
            case 123: s = "F12";break;
            case 46: s = "Del";break;
            case 45: s = "Ins";break;
            case 33: s = "PgUp";break;
            case 34: s = "PgDn";break;
            case 192: s = "~";break;
            case 48: s = "0";break;
            case 49: s = "1";break;
            case 50: s = "2";break;
            case 51: s = "3";break;
            case 52: s = "4";break;
            case 53: s = "5";break;
            case 54: s = "6";break;
            case 55: s = "7";break;
            case 56: s = "8";break;
            case 57: s = "9";break;
            case 189: s = "-";break;
            case 187: s = "=";break;
            case 8: s = "Back";break;
            case 144: s = "Num";break;
            case 111: s = "/";break;
            case 106: s = "*";break;
            case 109: s = "-";break;
            case 107: s = "+";break;
            case 9: s = "Tab";break;
            case 20: s = "CL";break;
            case 160: s = "Shift";break;
            case 162: s = "lCtrl";break;
            case 91: s = "Win";break;
            case 164: s = "lAlt";break;
            case 32: s = "Space";break;
            case 165: s = "rAlt";break;
            case 163: s = "rCtrl";break;
            case 219: s = "[";break;
            case 221: s = "]";break;
            case 220: s = "\\";break;
            case 186: s = ";";break;
            case 222: s = "'";break;
            case 13: s = "Enter";break;
            case 188: s = ",";break;
            case 190: s = ".";break;
            case 191: s = "/";break;
            case 161: s = "Shift";break;
            case 37: s = "←";break;
            case 38: s = "↑";break;
            case 39: s = "→";break;
            case 40: s = "↓";break;
            case 97: s = "1";break;
            case 98: s = "2";break;
            case 99: s = "3";break;
            case 100: s = "4";break;
            case 101: s = "5";break;
            case 102: s = "6";break;
            case 103: s = "7";break;
            case 104: s = "8";break;
            case 105: s = "9";break;
            case 96: s = "0";break;
            case 110: s = ".";break;
            case 999: s = "kps"; break;

            default: s = vcode.ToString(); break;

        }
            

        return s;
    }

    // 以下已转成独立的脚本，只在特定条件下运行
    //private void FixedUpdate() // 释放内存
    //{
    //    timer += Time.fixedDeltaTime;

    //    if (timer > 100f)
    //    {
    //        timer = 0;
    //        clearMemory();
    //    }
    //}
    //float f;
    //private void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer > 100f)
    //    {
    //        timer = 0;
    //        clearMemory();
    //        UnityEngine.Debug.Log("1");
    //    }

    //    if (!mainMenu.activeSelf)
    //    {
    //        return;
    //    }
    //    f = Input.GetAxis("Mouse ScrollWheel");
    //    if (f > 0 && btns[btns.Count - 1].activeSelf) // ▲
    //    {
    //        goUp();
    //    }
    //    else if (f < 0 && btns[btns.Count - 2].activeSelf)
    //    {
    //        goDown();
    //    }
    //}

    private void setSizePageActiveFalse()
    {
        opacityPage.SetActive(false);
        sizePage.SetActive(false);
        setAllWindowsOpacitySliderPage.SetActive(false);
        systemConditionPage.SetActive(false);
    }

}
