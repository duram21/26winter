using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// 퀘스트 목록 표시 UI (메이플스토리 스타일)
/// </summary>
public class QuestListUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject questListPanel;
    public TMP_Text titleText;
    public Transform questButtonContainer;
    public GameObject questButtonPrefab;
    public Button closeButton;

    private List<GameObject> activeButtons = new List<GameObject>();
    private Action<QuestData> onQuestSelected;

    void Start()
    {
        // 시작 시 패널 숨김
        if (questListPanel != null)
        {
            questListPanel.SetActive(false);
        }

        // 닫기 버튼
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideQuestList);
        }
    }

    /// <summary>
    /// 퀘스트 목록 표시
    /// </summary>
    /// <param name="quests">표시할 퀘스트 리스트</param>
    /// <param name="callback">퀘스트 선택 시 호출될 콜백</param>
    public void ShowQuestList(List<QuestData> quests, Action<QuestData> callback)
    {
        if (questListPanel == null || questButtonPrefab == null)
        {
            Debug.LogError("QuestListUI: UI 참조가 설정되지 않았습니다!");
            return;
        }

        if (quests == null || quests.Count == 0)
        {
            Debug.LogWarning("QuestListUI: 표시할 퀘스트가 없습니다!");
            HideQuestList();
            return;
        }

        // 이전 버튼 제거
        ClearButtons();

        // 제목 설정
        if (titleText != null)
        {
            titleText.text = "받을 수 있는 퀘스트";
        }

        // 콜백 저장
        onQuestSelected = callback;

        // 퀘스트 버튼 생성
        foreach (var quest in quests)
        {
            CreateQuestButton(quest);
        }

        // 패널 표시
        questListPanel.SetActive(true);
    }

    /// <summary>
    /// 퀘스트 버튼 생성
    /// </summary>
    void CreateQuestButton(QuestData quest)
    {
        GameObject buttonObj = Instantiate(questButtonPrefab, questButtonContainer);

        // 버튼 텍스트 설정
        TMP_Text titleText = buttonObj.transform.Find("TitleText")?.GetComponent<TMP_Text>();
        if (titleText != null)
        {
            titleText.text = quest.questTitle;
        }

        TMP_Text descText = buttonObj.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();
        if (descText != null)
        {
            // 첫 줄만 표시 (간략하게)
            descText.text = quest.description.Length > 50
                ? quest.description.Substring(0, 50) + "..."
                : quest.description;
        }

        // 버튼 클릭 이벤트
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            QuestData selectedQuest = quest; // 클로저를 위한 로컬 변수
            button.onClick.AddListener(() => OnQuestClicked(selectedQuest));
        }

        activeButtons.Add(buttonObj);
    }

    /// <summary>
    /// 퀘스트 클릭 시
    /// </summary>
    void OnQuestClicked(QuestData quest)
    {
        // 패널 숨김
        HideQuestList();

        // 콜백 호출
        onQuestSelected?.Invoke(quest);
    }

    /// <summary>
    /// 퀘스트 목록 숨김
    /// </summary>
    public void HideQuestList()
    {
        if (questListPanel != null)
        {
            questListPanel.SetActive(false);
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
    /// 퀘스트 목록이 활성화되어 있는지
    /// </summary>
    public bool IsActive()
    {
        return questListPanel != null && questListPanel.activeSelf;
    }
}
