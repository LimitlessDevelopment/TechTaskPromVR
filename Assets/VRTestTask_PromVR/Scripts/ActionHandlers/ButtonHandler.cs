using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ButtonHandler – шаг сценария, который считается выполненным, когда пользователь нажмёт UI‑кнопку.
/// </summary>
public class ButtonHandler : ActionHandler
{
    /// <summary>
    /// Ссылка на UI‑кнопку. Берём автоматически из компонента на том же объекте.
    /// </summary>
    Button _uiButton;

    void Start() => _uiButton.onClick.AddListener(OnButtonPressed); // Подписываемся на клик по UI-кнопке
    private void OnEnable() => _uiButton.onClick.AddListener(OnButtonPressed);// Подписываемся на клик по кнопке при каждом включении (безопасно вкл/выкл)
    private void OnDisable() => _uiButton.onClick.RemoveListener(OnButtonPressed);// Обязательно отписываемся, чтобы избежать утечки событий

    /// <summary>
    /// Обработчик клика по кнопке.
    /// </summary>
    private void OnButtonPressed()
    {
        // сообщаем TaskManagerʼу, что пользователь сделал действие
        FireTriggered();

        if (!isActiveAction)
        {
            // Кнопку нажали не в тот момент – фиксируем ошибку
            CompleteAction(withError: true);
            return;
        }

        // Шаг выполнен корректно
        CompleteAction(withError: false);
    }
}
