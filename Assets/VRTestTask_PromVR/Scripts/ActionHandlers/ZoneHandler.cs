using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZoneHandler : ActionHandler
{
    private void OnTriggerEnter(Collider other)
    {
        FireTriggered();
        if (!isActiveAction) return;
        // Предполагаем, что у XR Origin (игрока) проставлен тег "Player"
        if (!other.CompareTag("Player")) return;
            CompleteAction(withError: false);
    }

}
