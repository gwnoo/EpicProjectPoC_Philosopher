using UnityEngine;
using TMPro;

public class TurnDisplay : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager; // TurnManager 참조
    private TextMeshProUGUI turnTextTMP;

    void Start()
    {
        // TextMeshProUGUI 컴포넌트 가져오기
        turnTextTMP = GetComponent<TextMeshProUGUI>();
        if (turnTextTMP == null)
        {
            Debug.LogError("TurnDisplay에 TextMeshProUGUI 컴포넌트가 없습니다.");
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

        // 초기 턴 표시
        UpdateTurnDisplay();
    }

    void Update()
    {
        // 매 프레임마다 TurnManager의 현재 턴을 반영
        if (turnManager != null)
        {
            UpdateTurnDisplay();
        }
    }

    // 텍스트 업데이트
    private void UpdateTurnDisplay()
    {
        int currentTurn = turnManager != null ? turnManager.GetCurrentTurn() : 0;
        string displayText = "Turn: " + currentTurn;
        if (turnTextTMP != null)
        {
            turnTextTMP.text = displayText;
        }
    }

    // 현재 턴 반환
    public int GetCurrentTurn()
    {
        return turnManager != null ? turnManager.GetCurrentTurn() : 0;
    }
}