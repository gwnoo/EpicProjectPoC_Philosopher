using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager; // TurnManager 참조
    private TextMeshProUGUI timeTextTMP;

    void Start()
    {
        // TextMeshProUGUI 컴포넌트 가져오기
        timeTextTMP = GetComponent<TextMeshProUGUI>();
        if (timeTextTMP == null)
        {
            Debug.LogError("TimeDisplay에 TextMeshProUGUI 컴포넌트가 없습니다.");
        }

        // TurnManager 참조 확인
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
            if (turnManager == null)
            {
                Debug.LogError("TurnManager를 찾을 수 없습니다. 인스펙터에서 참조를 설정하세요.");
                return;
            }
        }

        // 초기 시간 표시
        UpdateTimeDisplay();
    }

    void Update()
    {
        // 매 프레임마다 TurnManager의 남은 시간을 반영
        if (turnManager != null)
        {
            UpdateTimeDisplay();
        }
    }

    // 텍스트 업데이트
    private void UpdateTimeDisplay()
    {
        float remainingTime = turnManager != null ? turnManager.GetRemainingTurnTime() : 0f;
        string displayText = remainingTime.ToString("F1"); // 소수점 1자리까지 표시
        if (timeTextTMP != null)
        {
            timeTextTMP.text = displayText;
        }
    }

    // 남은 시간 반환
    public float GetRemainingTime()
    {
        return turnManager != null ? turnManager.GetRemainingTurnTime() : 0f;
    }
}