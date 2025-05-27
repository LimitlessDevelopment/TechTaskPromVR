using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabHandler : ActionHandler
{
    private XRGrabInteractable _interactable;       // Ссылка на XRGrabInteractable объекта

    void Start()
    {
            _interactable = GetComponent<XRGrabInteractable>();

        // Подписываемся на событие захвата объекта
        _interactable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Вызывается, когда объект взят любым XR Interactor (рукой пользователя)
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
