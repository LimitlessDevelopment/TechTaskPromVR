using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZoneHandler : ActionHandler
{
    private void OnTriggerEnter(Collider other)
    {
        FireTriggered();
        if (!isActiveAction) return;
        // ������������, ��� � XR Origin (������) ���������� ��� "Player"
        if (!other.CompareTag("Player")) return;
            CompleteAction(withError: false);
    }

}
