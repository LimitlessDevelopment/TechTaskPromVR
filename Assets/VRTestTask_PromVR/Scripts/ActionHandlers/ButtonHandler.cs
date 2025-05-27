using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : ActionHandler
{
    Button _uiButton;

    void Start()
    {
        // ������������� �� ���� �� UI-������
        _uiButton.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        if (!isActiveAction)
        {
            // ������ ������ � ������������ ����� (��������, ������ ����)
            CompleteAction(withError: true);
            return;
        }
        CompleteAction(withError: false);
    }
}
