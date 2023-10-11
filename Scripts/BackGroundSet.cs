using UnityEngine;
using System.Runtime.InteropServices; // 为了使用DllImport
using System;



/// <summary>
/// 让程序背景透明
/// </summary>
public class BackGroundSet : MonoBehaviour
{
    private IntPtr hwnd;
    private int currentX;
    private int currentY;


    #region Win函数常量
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("Dwmapi.dll")]
    static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    /// <summary>
    ///设置窗体可穿透点击的透明.
    ///参数1:窗体句柄
    ///参数2:透明颜色  0为黑色,按照从000000到FFFFFF的颜色,转换为10进制的值
    ///参数3:透明度,设置成255就是全透明
    ///参数4:透明方式,1表示将该窗口颜色为[参数2]的部分设置为透明,2表示根据透明度设置窗体的透明度
    /// </summary>
    [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
    private static extern uint SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

    #region UpdateLayeredWindow Test
    //[StructLayout(LayoutKind.Sequential)]
    //public class Point
    //{
    //    public Int32 x;
    //    public Int32 y;

    //    public Point(Int32 x, Int32 y)
    //    {
    //        this.x = x;
    //        this.y = y;
    //    }
    //}
    //[StructLayout(LayoutKind.Sequential)]
    //public struct Size
    //{
    //    public Int32 cx;
    //    public Int32 cy;

    //    public Size(Int32 x, Int32 y)
    //    {
    //        cx = x;
    //        cy = y;
    //    }
    //}
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    // public struct BLENDFUNCTION
    // {
    //     public byte BlendOp;
    //     public byte BlendFlags;
    //     public byte SourceConstantAlpha;
    //     public byte AlphaFormat;
    //}
    //// UpdateLayeredWindow函数更新一个分层窗口的位置，大小，形状，内容和半透明度。
    //// hwnd : 一个分层窗口的句柄。分层窗口在用CreateWindowEx函数创建窗口时应指定WS_EX_LAYERED扩展样式。
    //// hdcDst : 屏幕的设备上下文(DC)句柄。如果指定为NULL，那么将会在调用函数时自己获得。它用来在窗口内容更新时与调色板颜色匹配。如果hdcDst为NULL，将会使用默认调色板。如果hdcSrc指定为NULL，那么hdcDst必须指定为NULL。
    //// pptDst : 指向分层窗口相对于屏幕的位置的POINT结构的指针。如果保持当前位置不变，pptDst可以指定为NULL。
    //// psize : 指向分层窗口的大小的SIZE结构的指针。如果窗口的大小保持不变，psize可以指定为NULL。如果hdcSrc指定为NULL，psize必须指定为NULL。
    //// hdcSrc : 分层窗口绘图表面的设备上下文句柄。这个句柄可以通过调用函数CreateCompatibleDC获得。如果窗口的形状和可视范围保持不变，hdcSrc可以指定为NULL。
    //// pptSrc : 指向分层窗口绘图表面在设备上下文位置的POINT结构的指针。如果hdcSrc指定为NULL，pptSrc就应该指定为NULL。
    //// crKey : 指定合成分层窗口时使用的颜色值。要生成一个类型为COLORREF的值，使用RGB宏。
    //// pblend : 指向指定合成分层窗口时使用的透明度结构的指针。
    //// dwFlags : 可以是以下值之一。如果hdcSrc指定为NULL，dwFlags应该指定为0。
    //// ULW_ALPHA  0x00000002
    //// 使用参数pblend作为混合函数,如果显示模式为256色或低于256色，使用这个值实现的效果和使用ULW_OPAQUE的效果相同。
    //// ULW_COLORKEY  0x00000001
    //// 使用参数crKey值作为透明颜色。
    //// ULW_OPAQUE  0x00000004
    //// 绘制一个不透明的分层窗口。
    //// 返回值类型：BOOL
    //[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    //public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);
    //[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    //public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
    //[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    //public static extern IntPtr GetDC(IntPtr hWnd);

    //private const Int32 ULW_COLORKEY = 0x00000001;
    //private const Int32 ULW_OPAQUE = 0x00000004;
    //public const byte AC_SRC_OVER = 0;
    //public const Int32 ULW_ALPHA = 2;
    //public const byte AC_SRC_ALPHA = 1;
    #endregion

    #region newWindow Test
    //[DllImport("user32.dll", CharSet = CharSet.Auto)]
    //public static extern IntPtr CreateWindowEx(
    //        int dwExStyle,                                //窗口的扩展风格
    //        string lpszClassName,                         //指向注册类名的指针
    //        string lpszWindowName,                        //指向窗口名称的指针
    //        int style,                                    //窗口风格
    //        int x,                                        //窗口的水平位置
    //        int y,                                        //窗口的垂直位置
    //        int width,                                    //窗口的宽度
    //        int height,                                   //窗口的高度
    //        IntPtr hWndParent,                            //父窗口的句柄
    //        IntPtr hMenu,                                 //菜单的句柄或是子窗口的标识符
    //        IntPtr hInst,                                 //应用程序实例的句柄
    //        [MarshalAs(UnmanagedType.AsAny)] object pvParam//指向窗口的创建数据
    //        );
    #endregion
    #region CreateNewWindow Test
    //private const int WS_CHILD = 0x40000000;
    //private const int WS_VISIBLE = 0x10000000;
    #endregion

    // 定义窗体样式,-16表示设定一个新的窗口风格
    private const int GWL_STYLE = -16;
    //设定一个新的扩展风格
    private const int GWL_EXSTYLE = -20;
    
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_CAPTION = 0x00C00000;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int LWA_COLORKEY = 0x00000001;
    private const int LWA_ALPHA = 0x00000002;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_TOPMOST = 0x00000008;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_COMPOSITED = 0x02000000;


    #endregion

    void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        // 注册表:\HKEY_CURRENT_USER\SOFTWARE\company name\product name  记录了分辨率大小
        // Screen.SetResolution(800, 600, false);
        var productName = Application.productName;
#if !UNITY_EDITOR
        // 获得窗口句柄
        hwnd = FindWindow(null, productName); 

        // 设置窗体属性
        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // 获得当前样式
        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_TOOLWINDOW); // 当前样式加上WS_EX_LAYERED     // WS_EX_TRANSPARENT 收不到点击的透明
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION); // 无边框、无标题栏

        // 设置窗体位置为右下角
        currentX = Screen.currentResolution.width - 900;
        currentY = Screen.currentResolution.height  - 800;
        SetWindowPos(hwnd, -1, currentX, currentY, 1200, 900, SWP_SHOWWINDOW); // Screen.currentResolution.width / 4 height...

        // 扩展窗口到客户端区域 -> 为了透明
        var margins = new MARGINS() { cxLeftWidth = -1 }; // 边距内嵌值确定在窗口四侧扩展框架的距离 -1为没有窗口边框
        DwmExtendFrameIntoClientArea(hwnd, ref margins);     

        // 将该窗口颜色为0的部分设置为透明,即背景可穿透点击，人物模型上不穿透
        SetLayeredWindowAttributes(hwnd, 0, 255, 1);
        //SetLayeredWindowAttributes(hwnd, 0, 255, 2); // 设为2时显示效果变好了，但不能穿透点击
#endif

        #region UpdateLayeredWindow Test
        //IntPtr screenDC = GetDC(IntPtr.Zero);
        //Point dc = new Point(currentX, currentY);
        //// 指向分层窗口的大小的SIZE结构的指针。如果窗口的大小保持不变，psize可以指定为NULL。如果hdcSrc指定为NULL，psize必须指定为NULL。
        //Size windowSize = new Size(1200, 900);
        //// 分层窗口绘图表面的设备上下文句柄。这个句柄可以通过调用函数CreateCompatibleDC获得。如果窗口的形状和可视范围保持不变，hdcSrc可以指定为NULL。
        //IntPtr memDc = CreateCompatibleDC(screenDC);
        //// 指向分层窗口绘图表面在设备上下文位置的POINT结构的指针。如果hdcSrc指定为NULL，pptSrc就应该指定为NULL。
        //Point srcLoc = new Point(0, 0);
        //// 指向指定合成分层窗口时使用的透明度结构的指针。
        //BLENDFUNCTION blendFunc = new BLENDFUNCTION();

        //blendFunc.BlendOp = AC_SRC_OVER;
        //blendFunc.SourceConstantAlpha = 255;
        //blendFunc.AlphaFormat = AC_SRC_ALPHA;
        //blendFunc.BlendFlags = 0;
        #endregion

        #region CreateNewWindow Test
        //IntPtr hwndnew = CreateWindowEx(0,
        //              "BUTTON",
        //              "中 心",
        //             (int)(WS_CHILD | WS_VISIBLE),
        //              400, 200, 300, 600,
        //              hwnd, IntPtr.Zero, IntPtr.Zero, null);

        //int intExTempNew = GetWindowLong(hwndnew, GWL_EXSTYLE); // 获得当前样式
        //SetWindowLong(hwndnew, GWL_EXSTYLE, intExTempNew & ~WS_EX_TRANSPARENT | WS_EX_COMPOSITED);
        //SetLayeredWindowAttributes(hwndnew, 0, 255, 1);

        #endregion

        //UpdateLayeredWindow(hwnd, screenDC, ref dc, ref windowSize, memDc, ref srcLoc, 0, ref blendFunc, ULW_OPAQUE);

        // 1
        // 调节窗体透明度可以先使用SetWindowLong为窗体加上WS_EX_LAYERED属性，
        // 再使用SetLayeredWindowAttributes来指定窗体的透明度。
        // 这样就可以在程序运行时动态的调节窗体的透明度了。
        // 2
        // 给 GWL_EXSTYLE 设置 WS_EX_TRANSPARENT 让窗口透明,能穿透点击，此时应用程序只能收到鼠标消息但收不到触摸消息（能点到程序同时点到桌面，点不到新出现的UI）
        // 3
        // 前面加上取反操作符"~"，就可以得到相反效果。比如，WS_CAPTION代表窗口有标题栏，~WS_CAPTION代表窗口没有标题栏
        // 4
        // GWL_STYLE指的是那些旧的窗口属性。相对于GWL_EXSTYLEGWL扩展属性而言的
        // 5
        // 要给窗口添加某属性，用 | 来连接，要去除某属性，用 & 来连接

        //创建按钮到外部程序
    }

    //public static void refresh() // 去掉穿透，为了能点击到程序内UI
    //{
    //    var hwnd = FindWindow(null, Application.productName);

    //    int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // 获得当前样式
    //    SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp & ~WS_EX_TRANSPARENT);
    //    SetLayeredWindowAttributes(hwnd, 0, 255, 2);
    //    //SetLayeredWindowAttributes(hwnd, 0, 255, 2);
    //}
    //public static void refreshEnd() // 加上穿透，程序闲置
    //{
    //    var hwnd = FindWindow(null, Application.productName);

    //    int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // 获得当前样式
    //    SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT);
    //    //SetLayeredWindowAttributes(hwnd, 0, 255, 2);
    //}
    //private void Update()
    //{
    //    hwnd = FindWindow(null, Application.productName);
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        hwnd = FindWindow(null, Application.productName);

    //        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
    //        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    //    }
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        hwnd = FindWindow(null, Application.productName);

    //        MARGINS margins = new MARGINS { cxLeftWidth = 0 };
    //        DwmExtendFrameIntoClientArea(hwnd, ref margins);

    //    }
    //}
}

