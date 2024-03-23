using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



#if (UNITY_EDITOR)
using UnityEditor;
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class Window : MonoBehaviour
{
    [SerializeField]
    [OnChangedCall("onSizeChanged")]
    public Vector2 Size = new Vector2(320, 240);
    [SerializeField]
    [OnChangedCall("onCaptionChanged")]
    public string Caption = "Window";
    [SerializeField]
    [OnChangedCall("onCaptionChanged")]
    public int CaptionFontSize = 64;
    [SerializeField]
    [OnChangedCall("onCaptionChanged")]
    public Color CaptionColor = Color.green;
    [SerializeField]
    [OnChangedCall("onMessageChanged")]
    public string Message = "Message";
    [SerializeField]
    [OnChangedCall("onMessageChanged")]
    public int MessageFontSize = 48;
    [SerializeField]
    [OnChangedCall("onMessageChanged")]
    public Color MessageColor = Color.black;
    [SerializeField]
    [OnChangedCall("onTipMessageChanged")]
    public string Tip = "Message";
    [SerializeField]
    [OnChangedCall("onTipMessageChanged")]
    public int TipFontSize = 48;
    [SerializeField]
    [OnChangedCall("onTipMessageChanged")]
    public Color TipColor = Color.black;
    [SerializeField]
    [OnChangedCall("onSpriteChanged")]
    public Sprite BgSprite;
    #region OnChangedMethods
    public void onSizeChanged()
    {
        GameObject parentGo = transform.Find("Panel")?.gameObject;
        if (!parentGo)
        {
            parentGo = new GameObject("Panel");
            parentGo.transform.parent = transform;

        };

        RectTransform parentRct = parentGo.GetComponent<RectTransform>();
        if (!parentRct)
        {
            parentRct = parentGo.AddComponent<RectTransform>();
            Image maskImg = parentRct.gameObject.AddComponent<Image>();
            Mask mask = parentRct.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;
        }
        parentRct.anchoredPosition = new Vector2(0, 0);
        parentRct.anchorMin = new Vector2(0.5f, 0.5f);
        parentRct.anchorMax = new Vector2(0.5f, 0.5f);
        parentRct.sizeDelta = Size;
    }
    public void onCaptionChanged()
    {
        GameObject parentGo = transform.Find("Panel")?.gameObject;
        if (!parentGo)
        {
            parentGo = new GameObject("Panel");
            parentGo.transform.parent = transform;
        };
        onSizeChanged();
        GameObject go = parentGo.transform.Find("Caption")?.gameObject;
        if (!go)
        {
            go = new GameObject("Caption");
            go.transform.parent = parentGo.transform;
        };
        go.transform.SetSiblingIndex(2);            
        TextMeshProUGUI txt = go.GetComponent<TextMeshProUGUI>();
        if (!txt)
        {
            txt = go.AddComponent<TextMeshProUGUI>();
            RectTransform rct = txt.GetComponent<RectTransform>();
            rct.pivot = new Vector2(0.5f, 1);
            rct.anchorMin = new Vector2(0, 1);
            rct.anchorMax = new Vector2(1, 1);
            rct.anchoredPosition = new Vector2(0, 0);
            rct.sizeDelta = new Vector2(0, 100);
            txt.enableAutoSizing = true;
            txt.alignment = TextAlignmentOptions.Center;
        }

        txt.text = Caption;
        txt.color = CaptionColor;
        txt.fontSize = CaptionFontSize;
        txt.fontSizeMax = CaptionFontSize;

    }
    public void onMessageChanged()
    {
        GameObject parentGo = transform.Find("Panel")?.gameObject;
        if (!parentGo)
        {
            parentGo = new GameObject("Panel");
            parentGo.transform.parent = transform;
        };
        onSizeChanged();
        GameObject go = parentGo.transform.Find("Message")?.gameObject;
        if (!go)
        {
            go = new GameObject("Message");
            go.transform.parent = parentGo.transform;
        };
        go.transform.SetSiblingIndex(1);
        TextMeshProUGUI txt = go.GetComponent<TextMeshProUGUI>();
        if (!txt)
        {
            txt = go.AddComponent<TextMeshProUGUI>();
            RectTransform rct = txt.GetComponent<RectTransform>();
            rct.pivot = new Vector2(0.5f, 0.5f);
            rct.anchorMin = new Vector2(0, 0.5f);
            rct.anchorMax = new Vector2(1, 0.5f);
            rct.anchoredPosition = new Vector2(0, 0);
            rct.sizeDelta = new Vector2(0, 100);
            txt.enableAutoSizing = true;
            txt.alignment = TextAlignmentOptions.Center;
        }
        txt.text = Message;
        txt.color = MessageColor;
        txt.fontSize = MessageFontSize;
        txt.fontSizeMax = MessageFontSize;
    }
    public void onTipMessageChanged()
    {
        GameObject parentGo = transform.Find("Panel")?.gameObject;
        if (!parentGo)
        {
            parentGo = new GameObject("Panel");
            parentGo.transform.parent = transform;
        };
        onSizeChanged();
        GameObject go = parentGo.transform.Find("Tip")?.gameObject;
        if (!go)
        {
            go = new GameObject("Tip");
            go.transform.parent = parentGo.transform;
        };
        go.transform.SetSiblingIndex(1);

        TextMeshProUGUI txt = go.GetComponent<TextMeshProUGUI>();
        if (!txt)
        {
            txt = go.AddComponent<TextMeshProUGUI>();
            RectTransform rct = txt.GetComponent<RectTransform>();
            rct.pivot = new Vector2(0.5f, 0.0f);
            rct.anchorMin = new Vector2(0, 0.0f);
            rct.anchorMax = new Vector2(1, 0.0f);
            rct.anchoredPosition = new Vector2(0, 0);
            rct.sizeDelta = new Vector2(0, 100);
            txt.enableAutoSizing = true;
            txt.alignment = TextAlignmentOptions.Center;
        }
        txt.text = Tip;
        txt.color = TipColor;
        txt.fontSize = TipFontSize;
        txt.fontSizeMax = TipFontSize;
    }
    public void onSpriteChanged()
    {
        GameObject parentGo = transform.Find("Panel")?.gameObject;
        if (!parentGo)
        {
            parentGo = new GameObject("Panel");
            parentGo.transform.parent = transform;

        };
        onSizeChanged();
        GameObject go = parentGo.transform.Find("Bg")?.gameObject;
        if (!go)
        {
            go = new GameObject("Bg");
            go.transform.parent = parentGo.transform;
        };

        go.transform.SetSiblingIndex(0);
        Image sp = go.GetComponent<Image>();
        if (!sp)
        {
            sp = go.AddComponent<Image>();
        }
        sp.type = Image.Type.Sliced;
        sp.sprite = BgSprite;
        AspectRatioFitter ft = go.GetComponent<AspectRatioFitter>();
        if (!ft)
        {
            ft = go.AddComponent<AspectRatioFitter>();
        }
        ft.aspectRatio = 1;
        ft.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
    }
    #endregion

    [SerializeField]
    public bool PauseGameWhileOpened = false;

    [SerializeField]
    public bool ShownAtStart = false;

    [SerializeField]
    public UnityEvent OnCloseAction = new UnityEvent();

#if (UNITY_EDITOR)
    void Reset()
    {
        checkCanvas();
        onCaptionChanged();
        onMessageChanged();
        onTipMessageChanged();
        onSpriteChanged();
    }
    private void checkCanvas() {
        if (!_canvas) {
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
#endif
    private Canvas _canvas;

#if UNITY_EDITOR
    void Awake()
    {
        if (!Application.isPlaying)
        {
            var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefabParent) { 
                if (transform.parent != null)
                {
                    var parentPrefabParent = PrefabUtility.GetCorrespondingObjectFromSource(transform.parent.gameObject);
                    if (parentPrefabParent)
                    {
                        throw new Exception("Window component prefab is in nested prefab. This behaviour is not allowed!");
                    }
                }
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
        }
    }
#endif
    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            if (PauseGameWhileOpened && ShownAtStart)
            {
                Time.timeScale = 0;
            }
        }
    }
    public void Open() {
        gameObject.SetActive(true);
        if (PauseGameWhileOpened)
        {
            Time.timeScale = 0;
        }
    }
    public void Close()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        OnCloseAction.Invoke();
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            gameObject.SetActive(ShownAtStart);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            Close();
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
