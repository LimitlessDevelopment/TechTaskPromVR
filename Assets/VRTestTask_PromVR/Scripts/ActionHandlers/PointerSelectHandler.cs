using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// PointerSelectHandler – шаг сценария, который считается выполненным,
/// когда пользователь «кликает» на объект лучом XR‑контроллера (selectEntered).
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class PointerSelectHandler : ActionHandler
{
    private XRSimpleInteractable _interactable; /// Ссылка на XRSimpleInteractable, к которому относится этот обработчик.

    private void Awake() => _interactable = GetComponent<XRSimpleInteractable>();// Получаем ссылку на интерактируемый объект (уже существует благодаря RequireComponent)

    private void OnEnable() => _interactable.selectEntered.AddListener(OnPointerSelect);// Подписываемся на событие при каждом включении компонента

    private void OnDisable() => _interactable.selectEntered.RemoveListener(OnPointerSelect);// Отписываемся при выключении, чтобы не копить слушателей

    /// <summary>
    /// Коллбэк, вызываемый XR Interaction Toolkit, когда объект выбран лучом.
    /// </summary>
    /// <param name="args">Информация о том, кто выбрал объект.</param>
    private void OnPointerSelect(SelectEnterEventArgs args)
    {
        // сообщаем менеджеру, что произошло действие
        FireTriggered();

        if (!isActiveAction)
        {
            // Кликнули на объект раньше, чем нужно
            CompleteAction(withError: true);
            return;
        }

        // Всё верно – шаг выполнен
        CompleteAction(withError: false);
    }
}
