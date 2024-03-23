using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if (UNITY_EDITOR)
using UnityEditor;
[ExecuteInEditMode]
#endif
public class Counter : MonoBehaviour
{    
    [SerializeField]
    [OnChangedCall("onCountChanged")]
    public int Count;
    [SerializeField]
    [OnChangedCall("onMaxCountChanged")]
    public int MaxCount;
    [SerializeField]
    [OnChangedCall("onTypeChanged")]
    public EUIWIdgetType WidgetType;
    [SerializeField]
    [OnChangedCall("onCornerChanged")]
    public EUIWidgetCorner Corner;
    [SerializeField]
    [OnChangedCall("onSpriteChanged")]
    public Sprite IconSprite;
    [SerializeField]
    [OnChangedCall("onSizeChanged")]
    public int Size = 32;
    [SerializeField]
    [OnChangedCall("onIconColorChanged")]
    public Color EmptyColor = new Color(128,128,128,128);
    [SerializeField]
    [OnChangedCall("onIconColorChanged")]
    public Color FullColor = new Color(255, 0, 0, 255);    
    [SerializeField]
    [OnChangedCall("onTextColorChanged")]
    public Color TextColor = new Color(255, 255, 255, 255);
    [SerializeField]
    [OnChangedCall("onTextFontChanged")]
    public Font TextFont;
    [SerializeField]
    [OnChangedCall("onFontSizeChanged")]
    public int FontSize = 32;
    [SerializeField]
    [OnChangedCall("onPaddingChanged")]
    public int padding = 10;
    [SerializeField]    
    [OnChangedCall("onPaddingChanged")]    
    public int verticalOffset = 10;

    private Canvas _canvas;
    private GridLayoutGroup _grid;

    public void SetCount(int count) {
        Count = count;
        onCountChanged();
    }

    public void SetMaxCount(int maxCount)
    {
        MaxCount = maxCount;
        onCountChanged();
    }

    public void AddCount()
    {
        Count++;
        onCountChanged();
    }

    #region OnChangedMethods
    public void onCountChanged()
    {
        onCountChangeHandler();
    }
    public void onMaxCountChanged()
    {
        onCountChangeHandler();
    }
    private void onCountChangeHandler()
    {
        checkCanvas();
        var tempList = _grid.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
        {
            if (Application.isPlaying)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            else
                GameObject.DestroyImmediate(child.gameObject);

        }

        switch (WidgetType)
        {
            case EUIWIdgetType.IconAndCounter:
                {
                    GameObject IconGo = new GameObject();
                    IconGo.name = "Icon";
                    IconGo.transform.parent = _grid.transform;
                    Image image = IconGo.AddComponent<Image>();

                    GameObject LabelGo = new GameObject();
                    LabelGo.name = "Count";
                    LabelGo.transform.parent = _grid.transform;
                    Text label = LabelGo.AddComponent<Text>();
                    label.text = Count.ToString();
                    label.resizeTextForBestFit = true;
                    label.alignment = TextAnchor.MiddleCenter;
                }
                break;
            case EUIWIdgetType.IconAndFixedCounter:
                {
                    GameObject IconGo = new GameObject();
                    IconGo.name = "Icon";
                    IconGo.transform.parent = _grid.transform;
                    Image image = IconGo.AddComponent<Image>();

                    GameObject LabelGo = new GameObject();
                    LabelGo.name = "Count";
                    LabelGo.transform.parent = _grid.transform;
                    Text label = LabelGo.AddComponent<Text>();
                    label.text = $"{Count}/{MaxCount}";
                    label.resizeTextForBestFit = true;
                    label.alignment = TextAnchor.MiddleCenter;
                }
                break;
            case EUIWIdgetType.FixedIcons:
                {
                    for (int i = 0; i < Mathf.Max(Count, MaxCount); i++)
                    {
                        GameObject go = new GameObject();
                        go.name = "Icon";
                        go.transform.parent = _grid.transform;
                        Image image = go.AddComponent<Image>();
                    }
                }
                break;
            case EUIWIdgetType.Icons:
                {
                    for (int i = 0; i < Count; i++)
                    {
                        GameObject go = new GameObject();
                        go.name = "Icon";
                        go.transform.parent = _grid.transform;
                        Image image = go.AddComponent<Image>();
                    }
                }
                break;
            default:
                break;
        }
        onSpriteChanged();
        onIconColorChanged();
        onFontSizeChanged();
        onTextColorChanged();
        onTextFontChanged();
    }
    public void onTypeChanged()
    {
        onCountChangeHandler();
    }
    public void onCornerChanged()
    {
        checkCanvas();
        var rect = _grid.GetComponent<RectTransform>();
        var grid = _grid;

        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = 1;
        switch (Corner)
        {
            case EUIWidgetCorner.TopRight:
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.one;
                rect.pivot = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
                grid.childAlignment = TextAnchor.UpperRight;
                break;
            case EUIWidgetCorner.TopLeft:
                rect.anchorMax = Vector2.up;
                rect.anchorMin = Vector2.up;
                rect.pivot = Vector2.up;
                rect.anchoredPosition = Vector2.zero;
                grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
                grid.childAlignment = TextAnchor.UpperLeft;
                break;
            case EUIWidgetCorner.BottomRight:
                rect.anchorMax = Vector2.right;
                rect.anchorMin = Vector2.right;
                rect.pivot = Vector2.right;
                rect.anchoredPosition = Vector2.zero;
                grid.startCorner = GridLayoutGroup.Corner.LowerLeft;
                grid.childAlignment = TextAnchor.LowerRight;
                break;
            case EUIWidgetCorner.BottomLeft:
                rect.anchorMax = Vector2.zero;
                rect.anchorMin = Vector2.zero;
                rect.pivot = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
                grid.startCorner = GridLayoutGroup.Corner.LowerLeft;
                grid.childAlignment = TextAnchor.LowerLeft;
                break;
            default:
                break;
        }
        onPaddingChanged();
        onFontSizeChanged();
    }
    public void onSpriteChanged()
    {
        checkCanvas();
        var images = _grid.GetComponentsInChildren<Image>().ToList().FindAll(img => img.gameObject.activeSelf);
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = IconSprite;
            images[i].GetComponent<RectTransform>().sizeDelta = new Vector2(Size, Size);
        }
    }
    public void onSizeChanged()
    {
        checkCanvas();
        var uiTransforms = _grid.GetComponentsInChildren<RectTransform>().ToList().FindAll(img => img.gameObject.activeSelf);
        for (int i = 0; i < uiTransforms.Count; i++)
        {
            uiTransforms[i].sizeDelta = new Vector2(Size, Size);
        }
        _grid.cellSize = new Vector2(Size, Size);
    }
    public void onIconColorChanged()
    {
        checkCanvas();
        Image[] images = _grid.GetComponentsInChildren<Image>().ToList().FindAll(img => img.gameObject.activeSelf).ToArray();

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = i < Count ? FullColor : EmptyColor;
        }
    }
    public void onTextColorChanged()
    {
        checkCanvas();
        Text[] texts = _grid.GetComponentsInChildren<Text>().ToList().FindAll(img => img.gameObject.activeSelf).ToArray();
        foreach (Text text in texts)
        {
            text.color = TextColor;
        }
    }
    public void onTextFontChanged()
    {
        checkCanvas();
        Text[] texts = _grid.GetComponentsInChildren<Text>().ToList().FindAll(img => img.gameObject.activeSelf).ToArray();
        foreach (Text text in texts)
        {
            text.font = TextFont;
        }
    }
    public void onFontSizeChanged()
    {
        checkCanvas();
        Text[] texts = _grid.GetComponentsInChildren<Text>().ToList().FindAll(img => img.gameObject.activeSelf).ToArray();
        foreach (Text text in texts)
        {
            text.fontSize = FontSize;
            text.resizeTextMaxSize = FontSize;
        }
    }
    public void onPaddingChanged()
    {
        checkCanvas();
        _grid.padding = new RectOffset(padding, padding, 0, 0);

        _grid.spacing = new Vector2(padding, padding);

        if (Corner >= EUIWidgetCorner.BottomRight)
        {
            _grid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, verticalOffset);
        }
        else
        {
            _grid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -verticalOffset);
        }
    }
    #endregion

#if (UNITY_EDITOR)
    void Awake()
    {
        if (!Application.isPlaying)
        {
            var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefabParent)
            {
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

    void Reset()
    {
        checkCanvas();
        onCornerChanged();
        onCountChangeHandler();
        onIconColorChanged();
    }
#endif

    private void checkCanvas()
    {
        if (!_canvas)
        {
            _canvas = transform.Find("Canvas")?.GetComponent<Canvas>();
            if (!_canvas)
            {
                var go = new GameObject("Canvas");
                go.transform.parent = transform;
                _canvas = go.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        if (!_grid)
        {
            _grid = _canvas.transform.Find("Grid")?.GetComponent<GridLayoutGroup>();
            if (!_grid)
            {
                var go = new GameObject("Grid");
                go.transform.parent = _canvas.transform;
                _grid = go.AddComponent<GridLayoutGroup>();
            }
        }
    }
}

[System.Serializable]
public enum EUIWidgetCorner {
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft
}

[System.Serializable]
public enum EUIWIdgetType
{
    IconAndCounter,
    IconAndFixedCounter,
    FixedIcons,
    Icons
}