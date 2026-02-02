using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// 대화 선택지 UI (메이플스토리 스타일)
/// </summary>
public class DialogueChoiceUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject choicePanel;
    public TMP_Text titleText;
    public Transform choiceButtonContainer;
    public GameObject choiceButtonPrefab;

    private List<GameObject> activeButtons = new List<GameObject>();
    private Action<int> onChoiceSelected;

    void Start()
    {
        // 시작 시 패널 숨김
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    /// <summary>
    /// 선택지 표시
    /// </summary>
    /// <param name="title">선택지 제목</param>
    /// <param name="choices">선택지 텍스트 목록</param>
    /// <param name="callback">선택 시 호출될 콜백 (선택한 인덱스 전달)</param>
    public void ShowChoices(string title, List<string> choices, Action<int> callback)
    {
        if (choicePanel == null || choiceButtonPrefab == null)
        {
            Debug.LogError("DialogueChoiceUI: UI 참조가 설정되지 않았습니다!");
            return;
        }

        // 이전 버튼 제거
        ClearButtons();

        // 제목 설정
        if (titleText != null)
        {
            titleText.text = title;
        }

        // 콜백 저장
        onChoiceSelected = callback;

        // 선택지 버튼 생성
        for (int i = 0; i < choices.Count; i++)
        {
            CreateChoiceButton(choices[i], i);
        }

        // 패널 표시
        choicePanel.SetActive(true);
    }

    /// <summary>
    /// 선택지 버튼 생성
    /// </summary>
    void CreateChoiceButton(string text, int index)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);

        // 버튼 텍스트 설정
        TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }

        // 버튼 클릭 이벤트
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            int choiceIndex = index; // 클로저를 위한 로컬 변수
            button.onClick.AddListener(() => OnChoiceClicked(choiceIndex));
        }

        activeButtons.Add(buttonObj);
    }

    /// <summary>
    /// 선택지 클릭 시
    /// </summary>
    void OnChoiceClicked(int index)
    {
        // 패널 숨김
        HideChoices();

        // 콜백 호출
        onChoiceSelected?.Invoke(index);
    }

    /// <summary>
    /// 선택지 숨김
    /// </summary>
    public void HideChoices()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        ClearButtons();
    }

    /// <summary>
    /// 버튼 제거
    /// </summary>
    void ClearButtons()
    {
        foreach (var button in activeButtons)
        {
            Destroy(button);
        }
        activeButtons.Clear();
    }

    /// <summary>
    /// 선택지가 활성화되어 있는지
    /// </summary>
    public bool IsActive()
    {
        return choicePanel != null && choicePanel.activeSelf;
    }

    public void initChoiceUI()    
    {
        
    }
}
