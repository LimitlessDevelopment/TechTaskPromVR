using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]    
public class PointerSelectHandler : ActionHandler
{
    private XRSimpleInteractable _interactable;

    void Start()
    {
        // ������������� �� ������� "������" ������� �����
        _interactable.selectEntered.AddListener(OnPointerSelect);
    }

    private void OnPointerSelect(SelectEnterEventArgs args)
    {
        if (!isActiveAction)
        {
            // �������� �� ������ ������, ��� �����
            CompleteAction(withError: true);
            return;
        }
        CompleteAction(withError: false);
    }
}
