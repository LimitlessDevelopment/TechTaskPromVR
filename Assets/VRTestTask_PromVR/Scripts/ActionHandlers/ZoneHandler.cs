using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZoneHandler : ActionHandler
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAction) return;
        // ������������, ��� � XR Origin (������) ���������� ��� "Player"
        if (other.CompareTag("Player"))
        {
            CompleteAction(withError: false);
        }
    }

}
