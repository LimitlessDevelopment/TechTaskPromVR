using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabHandler : ActionHandler
{
    private XRGrabInteractable _interactable;       // ������ �� XRGrabInteractable �������

    void Start()
    {
            _interactable = GetComponent<XRGrabInteractable>();

        // ������������� �� ������� ������� �������
        _interactable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // ����������, ����� ������ ���� ����� XR Interactor (����� ������������)
        if (!isActiveAction)
        {
            // ������ ����� ��� ������������������ (������ �������)
            CompleteAction(withError: true);
            return;
        }
        // ������ ���� � ������ ������
        CompleteAction(withError: false);
    }
}
