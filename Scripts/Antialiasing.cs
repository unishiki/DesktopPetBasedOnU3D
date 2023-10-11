using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Antialiasing : MonoBehaviour
{
    private static GameObject renderTargetCam;

    private static TextureRenderer textureRenderer;

    private static RenderTexture renderTexture;

    public static float scale = 2;

    private Camera mainCam;

    private int screenX;

    private int screenY;

    private int targetX = 100;

    private int targetY = 100;

    private int hideFlagDontShowSave = 61;

    public bool restart;

    private bool rendering;

    public static RenderTextureFormat Format = RenderTextureFormat.ARGB32;

    public static GameObject RenderTargetCamera
    {
        get
        {
            return Antialiasing.renderTargetCam;
        }
    }

    public static RenderTexture RenderTexture
    {
        get
        {
            return Antialiasing.renderTexture;
        }
    }
    public Camera RenderingCamera
    {
        get
        {
            return this.mainCam;
        }
    }
    private void OnEnable()
    {
        this.mainCam = base.GetComponent<Camera>();
        if (this.mainCam == null)
        {
            Debug.LogError("Missing Camera on GameObject!");
            base.enabled = false;
            return;
        }
        this.hideFlagDontShowSave = 13;
        this.targetX = Screen.width;
        this.targetY = Screen.height;
        //new Shader();
        if (Application.isEditor)
        {
            this.restart = true;
            return;
        }
        this.StartAntialiasing();
    }

    private void OnDisable()
    {
        this.StopAntialiasing();
    }

    private void Update()
    {
        if (this.screenX != Screen.width || this.screenY != Screen.height)
        {
            this.restart = true;
        }
        if (this.restart)
        {
            this.Restart();
        }
    }

    private void Restart()
    {
        this.StopAntialiasing();
        StartCoroutine(IERestart());
        this.restart = false;
    }

    private IEnumerator IERestart()
    {
        yield return new WaitForEndOfFrame();
        this.StartAntialiasing();
    }


    private void OnPreCull()
    {
        if (this.rendering && (this.screenX != Screen.width || this.screenY != Screen.height))
        {
            this.targetX = Screen.width;
            this.targetY = Screen.height;
            this.restart = true;
        }
        if (this.rendering)
        {
            this.mainCam.targetTexture = Antialiasing.renderTexture;
        }
    }

    private void FinishedRendering()
    {
        if (this.rendering)
        {
            this.mainCam.targetTexture = null;
        }
    }
    public void StartAntialiasing()
    {
        if (this.mainCam == null)
        {
            Debug.LogError("Missing Camera on Object!");
            return;
        }
        this.screenX = Screen.width;
        this.screenY = Screen.height;
        int num = (int)((float)Screen.width * Antialiasing.scale);
        int num2 = (int)((float)Screen.height * Antialiasing.scale);
        if (num <= 0)
        {
            num = 100;
        }
        if (num2 <= 0)
        {
            num2 = 100;
        }
        if (Antialiasing.renderTexture == null || Antialiasing.renderTexture.width != num || Antialiasing.renderTexture.height != num2)
        {
            if (Antialiasing.renderTexture != null)
            {
                Antialiasing.renderTexture.Release();
            }
            Antialiasing.renderTexture = new RenderTexture(num, num2, 2, Antialiasing.Format);
            Antialiasing.renderTexture.name = "SSAARenderTarget";
            Antialiasing.renderTexture.hideFlags = (HideFlags)this.hideFlagDontShowSave;
        }

        if (Antialiasing.renderTargetCam == null)
        {
            Antialiasing.renderTargetCam = new GameObject("SSAARenderTargetCamera");
            Antialiasing.renderTargetCam.hideFlags = (HideFlags)this.hideFlagDontShowSave;
            Camera c = Antialiasing.renderTargetCam.AddComponent<Camera>();
            c.CopyFrom(this.mainCam);
            c.cullingMask = 0;
            c.targetTexture = null;
            c.depth = this.mainCam.depth + 0.5f;
            Antialiasing.textureRenderer = Antialiasing.renderTargetCam.AddComponent<TextureRenderer>();
            Antialiasing.textureRenderer.hideFlags = (HideFlags)this.hideFlagDontShowSave;


        }
        this.rendering = true;
    }

    public void StopAntialiasing()
    {
        if (this.mainCam != null && this.mainCam.targetTexture != null)
        {
            this.mainCam.targetTexture = null;
        }
        if (renderTargetCam != null)
        {
            GameObject.Destroy(renderTargetCam);
        }
        this.rendering = false;

    }
}


public class TextureRenderer : MonoBehaviour
{
    private Camera c;
    public bool stereoFirstPass = true;
    private void Awake()
    {
        this.c = gameObject.GetComponent<Camera>();
        if (this.c == null)
        {
            Debug.LogError("TextureRenderer init fail! (no Camera)");
            base.enabled = false;
        }
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(Antialiasing.RenderTexture, destination);
        Antialiasing.RenderTexture.DiscardContents();
    }
}