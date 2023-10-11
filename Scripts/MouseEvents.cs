using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEvents : MonoBehaviour
{
    public static MouseEvents Instance;

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

        mainMenu.SetActive(false);
        sizePage.SetActive(false);
        opacityPage.SetActive(false);
        allWindowsOpacityPage.SetActive(false);
        SystemConditionPage.SetActive(false);
    }

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
    private const int VK_LBUTTON = 0x01; //������
    private const int VK_RBUTTON = 0x02; //����Ҽ�
    private const int VK_MBUTTON = 0x04; //����м�

    private bool _isLeftDown;
    private bool _isRightDown;
    private bool _isMiddleDown;

    public event Action<MouseKey, Vector3> MouseKeyDownEvent;
    public event Action<MouseKey, Vector3> MouseKeyUpEvent;
    public event Action<MouseKey, Vector3> MouseDragEvent;
    public event Action<MouseKey> MouseKeyClickEvent;

    public Vector3 MousePos { get; private set; }

    private bool _hasDragged;
    private Vector3 _leftDownPos;
    private Vector3 _rightDownPos;
    private Vector3 _middleDownPos;

    [SerializeField] private Animator anim;
    private GameObject player;
    // ====================================================================================== //

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    /// <summary>
    /// ����������Ļ�ϵ�λ��
    /// </summary>
    /// <param name="lpPoint"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(ref POINT lpPoint);


    /// <summary>
    /// ��¼��ǰ����λ��
    /// </summary>
    public POINT point;

    /// <summary>
    /// ����Ŀ�괰���С��λ��
    /// </summary>
    /// <param name="hWnd">Ŀ����</param>
    /// <param name="x">Ŀ�괰����λ��X������</param>
    /// <param name="y">Ŀ�괰����λ��Y������</param>
    /// <param name="nWidth">Ŀ�괰���¿��</param>
    /// <param name="nHeight">Ŀ�괰���¸߶�</param>
    /// <param name="BRePaint">�Ƿ�ˢ�´���</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);
    /// <summary>
    /// ��ǰ������
    /// </summary>
    private IntPtr hwnd;

    // ====================================================================================== //

    //[Header("�����ʽ")]
    //[SerializeField] private Texture2D normalCur;
    //[SerializeField] private Texture2D selectedCur;

    //private GameObject modle;
    public bool dragModle;
    [SerializeField] private Transform neck;

    [Header("�Ҽ����˵�")]
    [SerializeField] private GameObject mainMenu;
    [Header("���ô�Сҳ")]
    [SerializeField] private GameObject sizePage;
    [Header("����͸����ҳ")]
    [SerializeField] private GameObject opacityPage;
    [Header("�������д���͸����ҳ")]
    [SerializeField] private GameObject allWindowsOpacityPage;
    [Header("ϵͳ״̬ҳ")]
    [SerializeField] private GameObject SystemConditionPage;


    private void Start()
    {
        Init();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && mainMenu.activeSelf)
        {
            mainMenu.SetActive(false);
        }
        //if (Input.mousePosition.x < 400f / 2.3f || Input.mousePosition.x > Screen.width * 2.3f / 4f || Input.mousePosition.y < 90f || Input.mousePosition.y > Screen.height * 5.2f / 6f) // ģ��������(�ɴ�͸�������)
    }

    private void Update()
    {
        // �������
        if (GetAsyncKeyState(VK_LBUTTON) != 0)
        {
            // ���˵���SizePage��ʧ
            if (mainMenu.activeSelf && !EventSystem.current.IsPointerOverGameObject())
            {
                mainMenu.SetActive(false);
                // BackGroundSet.refreshEnd();
            }

            if (!_isLeftDown)
            {
                _isLeftDown = true;
                _leftDownPos = MouseKeyDown(MouseKey.Left);
            }
            else if (MousePos != Input.mousePosition)
            {
                MouseKeyDrag(MouseKey.Left);
                if (!_hasDragged)
                {
                    
                    

                    _hasDragged = true;

                    // ������קģ��
                    //if (Input.mousePosition.x > 400f / 2.3f && Input.mousePosition.x < Screen.width * 2.3f / 4f && Input.mousePosition.y > 90f && Input.mousePosition.y < Screen.height * 5.2f / 6f)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("showObj")) ||( EventSystem.current.IsPointerOverGameObject() && (UIScript.Instance.inGameMode || UIScript.Instance.inWorkMode || UIScript.Instance.inSimpleMode)))
                    {
                        dragModle = true;
                        if (!mainMenu.activeSelf)
                        {
                            anim.SetBool("pick", true);
                        }
                        mainMenu.SetActive(false);
                    }
                }
            }
        }
        // �����Ҽ�
        if (GetAsyncKeyState(VK_RBUTTON) != 0)
        {
            sizePage.SetActive(false);
            opacityPage.SetActive(false);
            SystemConditionPage.SetActive(false);
            allWindowsOpacityPage.SetActive(false);
            //Debug.Log(Input.mousePosition.x + "+" + Input.mousePosition.y);
            //mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 , Input.mousePosition.y * 3 , 0);

            // �Ҽ�ģ�ͳ������˵�
            if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("showObj")) || (EventSystem.current.IsPointerOverGameObject() && (UIScript.Instance.inGameMode || UIScript.Instance.inWorkMode || UIScript.Instance.inSimpleMode)))
            {
                if (point.X <= Screen.currentResolution.width / 2 && point.Y <= Screen.currentResolution.height / 2)
                {
                    mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 - 1500, Input.mousePosition.y * 3 - 1800, 0);
                }
                else if (point.X > Screen.currentResolution.width / 2 && point.Y <= Screen.currentResolution.height / 2)
                {
                    mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 - 1500 - 600, Input.mousePosition.y * 3 - 1800, 0);
                }
                else if (point.X <= Screen.currentResolution.width / 2 && point.Y > Screen.currentResolution.height / 2)
                {
                    mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 - 1500, Input.mousePosition.y * 3 - 1800 + 800, 0);
                }
                else if (point.X > Screen.currentResolution.width / 2 && point.Y > Screen.currentResolution.height / 2)
                {
                    mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 - 1500 - 600, Input.mousePosition.y * 3 - 1800 + 800, 0);
                }
                mainMenu.SetActive(true);
                //BackGroundSet.refresh();
                UIScript.Instance.onEnableSetUIPos();
            }


            if (!_isRightDown)
            {
                _isRightDown = true;
                _rightDownPos = MouseKeyDown(MouseKey.Right);
            }
            else if (MousePos != Input.mousePosition)
            {
                MouseKeyDrag(MouseKey.Right);
                if (!_hasDragged)
                {
                    _hasDragged = true;
                }
            }
        }
        // �����м�
        if (GetAsyncKeyState(VK_MBUTTON) != 0)
        {
            if (!_isMiddleDown)
            {
                _isMiddleDown = true;
                _middleDownPos = MouseKeyDown(MouseKey.Middle);
            }
            else if (MousePos != Input.mousePosition)
            {
                MouseKeyDrag(MouseKey.Middle);
                if (!_hasDragged)
                {
                    _hasDragged = true;
                }
            }
        }
        // ̧�����
        if (GetAsyncKeyState(VK_LBUTTON) == 0 && _isLeftDown)
        {
            _isLeftDown = false;
            MouseKeyUp(MouseKey.Left);

            // ����ק��down==up
            if (!_hasDragged && _leftDownPos == MousePos)
            {
                MouseKeyClick(MouseKey.Left);

                if(!anim.GetBool("bow") && !anim.GetBool("exit") && !UIScript.Instance.randomAnimMode && !UIScript.Instance.inGameMode && Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("showObj")))
                {
                    int num = UnityEngine.Random.Range(1, 3); // 1/2
                    neck.transform.rotation = Quaternion.Euler(0, 0, 0);
                    switch (num)
                    {
                        case 1:
                            GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("bow", true); break;
                        case 2:
                            GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("exit", true); break;
                    }
                }
               

                
            }

            _hasDragged = false;
            // ֹͣ��קģ��
            dragModle = false;
            anim.SetBool("pick", false);


        }
        // ̧���Ҽ�
        if (GetAsyncKeyState(VK_RBUTTON) == 0 && _isRightDown)
        {
            _isRightDown = false;
            MouseKeyUp(MouseKey.Right);

            if (!_hasDragged && _rightDownPos == MousePos)
            {
                MouseKeyClick(MouseKey.Right);
            }

            _hasDragged = false;
        }
        // ̧���м�
        if (GetAsyncKeyState(VK_MBUTTON) == 0 && _isMiddleDown)
        {
            _isMiddleDown = false;
            MouseKeyUp(MouseKey.Middle);

            if (!_hasDragged && _middleDownPos == MousePos)
            {
                MouseKeyClick(MouseKey.Middle);
            }

            _hasDragged = false;
        }

        #region ����
        // ��קʱ�ı���ײ���С
        //if (_hasDragged)
        //{
        //    modle.GetComponent<CapsuleCollider>().radius = 1.1f;
        //    modle.GetComponent<CapsuleCollider>().height = 2f;
        //}
        //else if (!_hasDragged)
        //{
        //    modle.GetComponent<CapsuleCollider>().radius = .12f;
        //    modle.GetComponent<CapsuleCollider>().height = 1.1f;
        //}


        // �����ʽ
        //if (!hasChanged && Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 5f, 1 << LayerMask.NameToLayer("showObj")))
        //{
        //    Cursor.SetCursor(selectedCur, Vector2.zero, CursorMode.Auto);
        //    hasChanged = true;
        //}
        //else if (hasChanged && Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit2, 5f, 1 << LayerMask.NameToLayer("showObj")))
        //{

        //}
        //else
        //{
        //    Cursor.SetCursor(normalCur, Vector2.zero, CursorMode.Auto);
        //    hasChanged = false;
        //}

        // �����ʽ2
        //if (cursorOnModel)
        //{
        //    Cursor.SetCursor(selectedCur, Vector2.zero, CursorMode.Auto);
        //}
        //else
        //{
        //    Cursor.SetCursor(normalCur, Vector2.zero, CursorMode.Auto);
        //}


        // ��ק

        //if ((Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 5f, 1 << LayerMask.NameToLayer("showObj"))) || (Input.mousePosition.x > 400f/2.3f && Input.mousePosition.x < Screen.width * 2.3f / 4f && Input.mousePosition.y > 90f && Input.mousePosition.y < Screen.height * 5.2f / 6f))
        //{
        //    if (_isLeftDown && _hasDragged)
        //    {
        //        GetCursorPos(ref point);//��ȡ�������Ļ�ϵ�λ��.�����������unity�е�λ��

        //        // �ڰ��µ�ʱ���¼���µ�����,Ȼ��������ϵ���ʱ���������������ƫ��
        //        // �ƶ�����
        //        // ����:
        //        // xָ����CWnd����ߵ���λ�á�
        //        // yָ����CWnd�Ķ�������λ�á�
        //        // nWidthָ����CWnd���¿�ȡ�
        //        // nHeightָ����CWnd���¸߶ȡ�
        //        // bRepaintָ�����Ƿ�Ҫ�ػ�CWnd
        //        // MoveWindow(hwnd, point.X - (int)Input.mousePosition.x, point.Y - (Screen.height  - (int)Input.mousePosition.y) + 1, Screen.width, Screen.height, true);
        //        MoveWindow(hwnd, point.X - Screen.width / 2, point.Y - Screen.height / 2, Screen.width, Screen.height, true);

        //        //Debug.Log("Input.mousePosition :" + Input.mousePosition);
        //        //Debug.Log("Point :" + point.X + "|" + point.Y);
        //        //Debug.Log(point.X + "|" + (int)Input.mousePosition.x);
        //    }


        //}
        #endregion
        // ��ק
        if (dragModle)
        {
            GetCursorPos(ref point);//��ȡ�������Ļ�ϵ�λ��.�����������unity�е�λ��
            if (player.activeSelf)
            {
                MoveWindow(hwnd, point.X - Screen.width / 2 + 50, point.Y - Screen.height / 2, Screen.width, Screen.height, true);
            }
            else
            {
                MoveWindow(hwnd, point.X - Screen.width / 2 , point.Y - Screen.height / 2, Screen.width, Screen.height, true);
            }
           


        }
    }

    //private void Update()
    //{
    //    if (Input.mousePosition.y > Screen.height) Debug.Log("1");
    //    if (Input.mousePosition.y < 0) Debug.Log("2");
    //}


    public void Init()
    {
        _isLeftDown = false;
        _isRightDown = false;
        _isMiddleDown = false;
        _hasDragged = false;
        hwnd = FindWindow(null, Application.productName);
        dragModle = false;
        player = GameObject.FindWithTag("Player");

        point.X = Screen.currentResolution.width;
        point.Y = Screen.currentResolution.height;
    }


    private Vector3 MouseKeyDown(MouseKey mouseKey)
    {
        RefreshMousePos();
        MouseKeyDownEvent?.Invoke(mouseKey, MousePos);

        return MousePos;
    }
    private Vector3 MouseKeyUp(MouseKey mouseKey)
    {
        RefreshMousePos();
        MouseKeyUpEvent?.Invoke(mouseKey, MousePos);

        return MousePos;
    }

    private Vector3 MouseKeyDrag(MouseKey mouseKey)
    {
        RefreshMousePos();
        MouseDragEvent?.Invoke(mouseKey, MousePos);

        return MousePos;
    }

    private void MouseKeyClick(MouseKey mouseKey)
    {
        MouseKeyClickEvent?.Invoke(mouseKey);
    }

    private Vector3 RefreshMousePos()
    {
        MousePos = Input.mousePosition;
        return MousePos;
    }

    public Vector3 MousePosToWorldPos(Vector3 mousePos)
    {
        var screenInWorldPos = Camera.main.WorldToScreenPoint(mousePos);
        var refPos = new Vector3(mousePos.x, mousePos.y, screenInWorldPos.z);
        var pos = Camera.main.ScreenToWorldPoint(refPos);
        return pos;
    }
}

public enum MouseKey
{
    None,
    Left,
    Right,
    Middle
}

public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override string ToString()
    {
        return ("X:" + X + ", Y:" + Y);
    }

    
}

