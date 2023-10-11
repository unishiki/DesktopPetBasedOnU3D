using UnityEngine;
using System.Runtime.InteropServices; // Ϊ��ʹ��DllImport
using System;



/// <summary>
/// �ó��򱳾�͸��
/// </summary>
public class BackGroundSet : MonoBehaviour
{
    private IntPtr hwnd;
    private int currentX;
    private int currentY;


    #region Win��������
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
    ///���ô���ɴ�͸�����͸��.
    ///����1:������
    ///����2:͸����ɫ  0Ϊ��ɫ,���մ�000000��FFFFFF����ɫ,ת��Ϊ10���Ƶ�ֵ
    ///����3:͸����,���ó�255����ȫ͸��
    ///����4:͸����ʽ,1��ʾ���ô�����ɫΪ[����2]�Ĳ�������Ϊ͸��,2��ʾ����͸�������ô����͸����
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
    //// UpdateLayeredWindow��������һ���ֲ㴰�ڵ�λ�ã���С����״�����ݺͰ�͸���ȡ�
    //// hwnd : һ���ֲ㴰�ڵľ�����ֲ㴰������CreateWindowEx������������ʱӦָ��WS_EX_LAYERED��չ��ʽ��
    //// hdcDst : ��Ļ���豸������(DC)��������ָ��ΪNULL����ô�����ڵ��ú���ʱ�Լ���á��������ڴ������ݸ���ʱ���ɫ����ɫƥ�䡣���hdcDstΪNULL������ʹ��Ĭ�ϵ�ɫ�塣���hdcSrcָ��ΪNULL����ôhdcDst����ָ��ΪNULL��
    //// pptDst : ָ��ֲ㴰���������Ļ��λ�õ�POINT�ṹ��ָ�롣������ֵ�ǰλ�ò��䣬pptDst����ָ��ΪNULL��
    //// psize : ָ��ֲ㴰�ڵĴ�С��SIZE�ṹ��ָ�롣������ڵĴ�С���ֲ��䣬psize����ָ��ΪNULL�����hdcSrcָ��ΪNULL��psize����ָ��ΪNULL��
    //// hdcSrc : �ֲ㴰�ڻ�ͼ������豸�����ľ��������������ͨ�����ú���CreateCompatibleDC��á�������ڵ���״�Ϳ��ӷ�Χ���ֲ��䣬hdcSrc����ָ��ΪNULL��
    //// pptSrc : ָ��ֲ㴰�ڻ�ͼ�������豸������λ�õ�POINT�ṹ��ָ�롣���hdcSrcָ��ΪNULL��pptSrc��Ӧ��ָ��ΪNULL��
    //// crKey : ָ���ϳɷֲ㴰��ʱʹ�õ���ɫֵ��Ҫ����һ������ΪCOLORREF��ֵ��ʹ��RGB�ꡣ
    //// pblend : ָ��ָ���ϳɷֲ㴰��ʱʹ�õ�͸���Ƚṹ��ָ�롣
    //// dwFlags : ����������ֵ֮һ�����hdcSrcָ��ΪNULL��dwFlagsӦ��ָ��Ϊ0��
    //// ULW_ALPHA  0x00000002
    //// ʹ�ò���pblend��Ϊ��Ϻ���,�����ʾģʽΪ256ɫ�����256ɫ��ʹ�����ֵʵ�ֵ�Ч����ʹ��ULW_OPAQUE��Ч����ͬ��
    //// ULW_COLORKEY  0x00000001
    //// ʹ�ò���crKeyֵ��Ϊ͸����ɫ��
    //// ULW_OPAQUE  0x00000004
    //// ����һ����͸���ķֲ㴰�ڡ�
    //// ����ֵ���ͣ�BOOL
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
    //        int dwExStyle,                                //���ڵ���չ���
    //        string lpszClassName,                         //ָ��ע��������ָ��
    //        string lpszWindowName,                        //ָ�򴰿����Ƶ�ָ��
    //        int style,                                    //���ڷ��
    //        int x,                                        //���ڵ�ˮƽλ��
    //        int y,                                        //���ڵĴ�ֱλ��
    //        int width,                                    //���ڵĿ��
    //        int height,                                   //���ڵĸ߶�
    //        IntPtr hWndParent,                            //�����ڵľ��
    //        IntPtr hMenu,                                 //�˵��ľ�������Ӵ��ڵı�ʶ��
    //        IntPtr hInst,                                 //Ӧ�ó���ʵ���ľ��
    //        [MarshalAs(UnmanagedType.AsAny)] object pvParam//ָ�򴰿ڵĴ�������
    //        );
    #endregion
    #region CreateNewWindow Test
    //private const int WS_CHILD = 0x40000000;
    //private const int WS_VISIBLE = 0x10000000;
    #endregion

    // ���崰����ʽ,-16��ʾ�趨һ���µĴ��ڷ��
    private const int GWL_STYLE = -16;
    //�趨һ���µ���չ���
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
        // ע���:\HKEY_CURRENT_USER\SOFTWARE\company name\product name  ��¼�˷ֱ��ʴ�С
        // Screen.SetResolution(800, 600, false);
        var productName = Application.productName;
#if !UNITY_EDITOR
        // ��ô��ھ��
        hwnd = FindWindow(null, productName); 

        // ���ô�������
        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // ��õ�ǰ��ʽ
        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_TOOLWINDOW); // ��ǰ��ʽ����WS_EX_LAYERED     // WS_EX_TRANSPARENT �ղ��������͸��
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION); // �ޱ߿��ޱ�����

        // ���ô���λ��Ϊ���½�
        currentX = Screen.currentResolution.width - 900;
        currentY = Screen.currentResolution.height  - 800;
        SetWindowPos(hwnd, -1, currentX, currentY, 1200, 900, SWP_SHOWWINDOW); // Screen.currentResolution.width / 4 height...

        // ��չ���ڵ��ͻ������� -> Ϊ��͸��
        var margins = new MARGINS() { cxLeftWidth = -1 }; // �߾���Ƕֵȷ���ڴ����Ĳ���չ��ܵľ��� -1Ϊû�д��ڱ߿�
        DwmExtendFrameIntoClientArea(hwnd, ref margins);     

        // ���ô�����ɫΪ0�Ĳ�������Ϊ͸��,�������ɴ�͸���������ģ���ϲ���͸
        SetLayeredWindowAttributes(hwnd, 0, 255, 1);
        //SetLayeredWindowAttributes(hwnd, 0, 255, 2); // ��Ϊ2ʱ��ʾЧ������ˣ������ܴ�͸���
#endif

        #region UpdateLayeredWindow Test
        //IntPtr screenDC = GetDC(IntPtr.Zero);
        //Point dc = new Point(currentX, currentY);
        //// ָ��ֲ㴰�ڵĴ�С��SIZE�ṹ��ָ�롣������ڵĴ�С���ֲ��䣬psize����ָ��ΪNULL�����hdcSrcָ��ΪNULL��psize����ָ��ΪNULL��
        //Size windowSize = new Size(1200, 900);
        //// �ֲ㴰�ڻ�ͼ������豸�����ľ��������������ͨ�����ú���CreateCompatibleDC��á�������ڵ���״�Ϳ��ӷ�Χ���ֲ��䣬hdcSrc����ָ��ΪNULL��
        //IntPtr memDc = CreateCompatibleDC(screenDC);
        //// ָ��ֲ㴰�ڻ�ͼ�������豸������λ�õ�POINT�ṹ��ָ�롣���hdcSrcָ��ΪNULL��pptSrc��Ӧ��ָ��ΪNULL��
        //Point srcLoc = new Point(0, 0);
        //// ָ��ָ���ϳɷֲ㴰��ʱʹ�õ�͸���Ƚṹ��ָ�롣
        //BLENDFUNCTION blendFunc = new BLENDFUNCTION();

        //blendFunc.BlendOp = AC_SRC_OVER;
        //blendFunc.SourceConstantAlpha = 255;
        //blendFunc.AlphaFormat = AC_SRC_ALPHA;
        //blendFunc.BlendFlags = 0;
        #endregion

        #region CreateNewWindow Test
        //IntPtr hwndnew = CreateWindowEx(0,
        //              "BUTTON",
        //              "�� ��",
        //             (int)(WS_CHILD | WS_VISIBLE),
        //              400, 200, 300, 600,
        //              hwnd, IntPtr.Zero, IntPtr.Zero, null);

        //int intExTempNew = GetWindowLong(hwndnew, GWL_EXSTYLE); // ��õ�ǰ��ʽ
        //SetWindowLong(hwndnew, GWL_EXSTYLE, intExTempNew & ~WS_EX_TRANSPARENT | WS_EX_COMPOSITED);
        //SetLayeredWindowAttributes(hwndnew, 0, 255, 1);

        #endregion

        //UpdateLayeredWindow(hwnd, screenDC, ref dc, ref windowSize, memDc, ref srcLoc, 0, ref blendFunc, ULW_OPAQUE);

        // 1
        // ���ڴ���͸���ȿ�����ʹ��SetWindowLongΪ�������WS_EX_LAYERED���ԣ�
        // ��ʹ��SetLayeredWindowAttributes��ָ�������͸���ȡ�
        // �����Ϳ����ڳ�������ʱ��̬�ĵ��ڴ����͸�����ˡ�
        // 2
        // �� GWL_EXSTYLE ���� WS_EX_TRANSPARENT �ô���͸��,�ܴ�͸�������ʱӦ�ó���ֻ���յ������Ϣ���ղ���������Ϣ���ܵ㵽����ͬʱ�㵽���棬�㲻���³��ֵ�UI��
        // 3
        // ǰ�����ȡ��������"~"���Ϳ��Եõ��෴Ч�������磬WS_CAPTION�������б�������~WS_CAPTION������û�б�����
        // 4
        // GWL_STYLEָ������Щ�ɵĴ������ԡ������GWL_EXSTYLEGWL��չ���Զ��Ե�
        // 5
        // Ҫ���������ĳ���ԣ��� | �����ӣ�Ҫȥ��ĳ���ԣ��� & ������

        //������ť���ⲿ����
    }

    //public static void refresh() // ȥ����͸��Ϊ���ܵ����������UI
    //{
    //    var hwnd = FindWindow(null, Application.productName);

    //    int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // ��õ�ǰ��ʽ
    //    SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp & ~WS_EX_TRANSPARENT);
    //    SetLayeredWindowAttributes(hwnd, 0, 255, 2);
    //    //SetLayeredWindowAttributes(hwnd, 0, 255, 2);
    //}
    //public static void refreshEnd() // ���ϴ�͸����������
    //{
    //    var hwnd = FindWindow(null, Application.productName);

    //    int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE); // ��õ�ǰ��ʽ
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

