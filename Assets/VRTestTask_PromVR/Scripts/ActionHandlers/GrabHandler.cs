using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// GrabHandler – шаг сценария, считающийся выполненным, когда игрок берёт (selectEntered)
/// указанный XRGrabInteractable объект.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class GrabHandler : ActionHandler
{
    // XRGrabInteractable, который должен быть захвачен. Берётся автоматом из объекта.
    private XRGrabInteractable _interactable;

    private void Awake() => _interactable = GetComponent<XRGrabInteractable>(); // Гарантированно присутствует благодаря атрибуту RequireComponent
    private void OnEnable() => _interactable.selectEntered.AddListener(OnGrabbed);// Подписываемся на событие при каждом включении компонента
    private void OnDisable() => _interactable.selectEntered.RemoveListener(OnGrabbed);// Отписываемся, чтобы избежать утечек событий

    /// <summary>
    /// Коллбэк, срабатывающий, когда объект захвачен XR‑контроллером.
    /// </summary>
    /// <param name="args">Содержит информацию о том, кто захватил объект.</param>
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Уведомляем TaskManager о действии пользователя
        FireTriggered();

        // Вызывается, когда объект взят любым XR Interactor
        if (!isActiveAction)
        {
            // Объект взяли вне последовательности (раньше времени)
            CompleteAction(withError: true);
            return;
        }
        // Объект взят в нужный момент
        CompleteAction(withError: false);
    }
}
