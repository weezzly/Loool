using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if (UNITY_EDITOR)
using UnityEditor;
[ExecuteInEditMode]
#endif
public class InteractiveButton : MonoBehaviour
{
    public UnityEvent action;
    public bool destroyAfterExecute = false;
    
    [Header("Condition")]
    [OnChangedCall(nameof(InitVisuals))]
    public bool needsCondition;
    [OnChangedCall(nameof(InitVisuals))]
    private int _itemsCurrent;
    [OnChangedCall(nameof(InitVisuals))]
    public int itemsToExecute = 3;

    public bool autoExecute = false;
    
    
    private const KeyCode ButtonToPress = KeyCode.E;
    
    private readonly Color _pressedRecolor = new Color(0.6f, 0.6f, 0.6f, 1f);
    
    
    private bool _playerIsNearby;

    
    private Transform _canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (!_canvasTransform)
                _canvasTransform = transform.GetChild(0);
            return _canvasTransform;
        }
    }
    
    private const float CanvasScaleSpeed = 5;
    private const float CanvasScaleMin = .75f;
    private const float CanvasWiggleSpeed = 5f;
    private const float CanvasWiggleAmp = .1f;

    private const float InactiveOpacity = .25f;

    private TextMeshProUGUI _starsCounterText;
    private TextMeshProUGUI StarsCounterText
    {
        get
        {
            if (!_starsCounterText)
                _starsCounterText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            return _starsCounterText;
        }
    }

    private RawImage _starsBg;
    private RawImage StarsBg
    {
        get
        {
            if (!_starsBg)
                _starsBg = transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
            return _starsBg;
        }
    }
    
    private RawImage _executeIcon;
    private RawImage ExecuteIcon
    {
        get
        {
            if (!_executeIcon)
                _executeIcon = transform.GetChild(0).GetChild(1).GetComponent<RawImage>();
            return _executeIcon;
        }
    }
    private RawImage _starsIcon;
    private RawImage StarsIcon
    {
        get
        {
            if (!_starsIcon)
                _starsIcon = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<RawImage>();
            return _starsIcon;
        }
    }
    
    private CanvasGroup _canvasGroupObject;
    private CanvasGroup CanvasGroupObject
    {
        get
        {
            if (!_canvasGroupObject)
                _canvasGroupObject = transform.GetChild(0).GetComponent<CanvasGroup>();
            return _canvasGroupObject;
        }
    }


    private bool ExecuteIconNeedsToBeShown => !(needsCondition && (_itemsCurrent < itemsToExecute));

    public void InitVisuals()
    {
        _itemsCurrent = Mathf.Clamp(_itemsCurrent, 0, itemsToExecute);
        StarsCounterText.text = _itemsCurrent + "/" + itemsToExecute;
        StarsBg.gameObject.SetActive(!ExecuteIconNeedsToBeShown);
        ExecuteIcon.gameObject.SetActive(ExecuteIconNeedsToBeShown);
    }



    void Awake()
    {
#if UNITY_EDITOR
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
#endif
        InitVisuals();
    }



    private void Update()
    {
        UpdateCanvasScaling();
        UpdateOpacity();
        
        UpdatePressButton();
    }

    private void UpdateCanvasScaling()
    {
        CanvasTransform.localScale = Vector3.Lerp(
            CanvasTransform.localScale,
            Vector3.one * (_playerIsNearby
                ? 1 + Mathf.Sin(Time.time * CanvasWiggleSpeed) * CanvasWiggleAmp
                : CanvasScaleMin),
            Time.deltaTime * CanvasScaleSpeed);
    }

    private void UpdateOpacity()
    {
        CanvasGroupObject.alpha = Mathf.Lerp(
            CanvasGroupObject.alpha,
            _playerIsNearby ? 1 : InactiveOpacity,
            Time.deltaTime * CanvasScaleSpeed);
        
    }

    private void UpdatePressButton()
    {
        if (!_playerIsNearby) return;
        if (!ExecuteIcon.gameObject.activeSelf) return;
        if (!Input.GetKeyDown(ButtonToPress)) return;
        Execute();
        ExecuteIcon.color = _pressedRecolor;
        Invoke(nameof(MakeColorWhite), .1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<PlatformerCharacterController>()) return;
        _playerIsNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<PlatformerCharacterController>()) return;
        _playerIsNearby = false;
    }
    
    private void MakeColorWhite()
    {
        ExecuteIcon.color = Color.white;
    }
    
    public void ToggleEnabledOfComponent(MonoBehaviour component)
    {
        component.enabled = !component.enabled;
    }

    public void AddItem(int amountOfItems)
    {
        _itemsCurrent += amountOfItems;
        InitVisuals();
        if (_itemsCurrent < itemsToExecute) return;
        if (!autoExecute) return;
        Execute();
    }

    private void Execute()
    {
        action?.Invoke();
        if (!destroyAfterExecute) return;
        Destroy(gameObject);
    }

}
