using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VHierarchy.Libs;

public class SkillXpPopup : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image radialProgressBar;
    [SerializeField] private TextMeshProUGUI cumulativeXpLabel;
    [SerializeField] private TextMeshProUGUI singleActionXpLabel;
    [SerializeField] private float activeDuration = 5f;
    
    [Header("Animation Settings")]
    [SerializeField] private float moveDistance = 100f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private UIFader _fader;
    private bool _fadeInFinished;
    private float _cumulativeXp;
    private Vector2 _initialActionXpLabelPosition;
    private RectTransform _labelRect;
    private SkillType _activeSkillType;
    private float _activeTimer;

    private void Awake()
    {
        _fader = GetComponent<UIFader>();
        _labelRect = singleActionXpLabel.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _fader.OnFadeInFinished += FadeInFinished;
        SkillManager.Instance.OnGainedAnyXp += XpGained;
    }

    private void OnDisable()
    {
        _fader.OnFadeInFinished -= FadeInFinished;
        SkillManager.Instance.OnGainedAnyXp -= XpGained;
    }

    private void Start()
    {
        _initialActionXpLabelPosition = _labelRect.anchoredPosition;
    }

    public void Init(SkillType skillType, float initialXpDrop)
    {
        _activeSkillType = skillType;
        _cumulativeXp += initialXpDrop;
        
        skillIcon.sprite = UiManager.Instance.uiTextureLibrary.skillIcons[_activeSkillType];
        radialProgressBar.color = UiManager.Instance.uiColorData.skillTextColors[_activeSkillType];

        float currentXp = SkillManager.Instance.Skills[_activeSkillType].CurrentLevelXp;
        float totalXp = SkillManager.Instance.Skills[_activeSkillType].TotalXp;
        radialProgressBar.fillAmount = currentXp / SkillManager.Instance.skillData.GetXpToNextLevel(totalXp);
    }

    void Update()
    {
        _activeTimer += Time.deltaTime;
        
        if (!_fadeInFinished)
            return;

        if (_activeTimer >= activeDuration)
            Destroy(gameObject);
    }
    
    private void XpGained(SkillType skillType, float xpGained)
    {
        _cumulativeXp += xpGained;
        PlayFloatUp();
    }
    
    public void PlayFloatUp()
    {
        _labelRect.anchoredPosition = _initialActionXpLabelPosition;
        StartCoroutine(FloatUpCoroutine());
    }

    private IEnumerator FloatUpCoroutine()
    {
        Vector2 startPos = _labelRect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, moveDistance);

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            _labelRect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, easingCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }

        _labelRect.anchoredPosition = endPos;
    }

    private void FadeInFinished()
    {
        _fadeInFinished = true;
    }
}
