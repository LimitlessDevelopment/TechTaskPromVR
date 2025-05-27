using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : ActionHandler
{
    Button _uiButton;

    void Start()
    {
        // Подписываемся на клик по UI-кнопке
        _uiButton.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        if (!isActiveAction)
        {
            // Нажали кнопку в неположенное время (например, раньше шага)
            CompleteAction(withError: true);
            return;
        }
        CompleteAction(withError: false);
    }
}
