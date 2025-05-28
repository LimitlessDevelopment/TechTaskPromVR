using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]    
public class PointerSelectHandler : ActionHandler
{
    private XRSimpleInteractable _interactable;

    void Start()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        // Подписываемся на событие "выбора" объекта лучом
        _interactable.selectEntered.AddListener(OnPointerSelect);
    }

    private void OnPointerSelect(SelectEnterEventArgs args)
    {
        FireTriggered();
        if (!isActiveAction)
        {
            // Кликнули на объект раньше, чем нужно
            CompleteAction(withError: true);
            return;
        }
        CompleteAction(withError: false);
    }
}
