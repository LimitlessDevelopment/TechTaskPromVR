using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// ZoneHandler – шаг сценария, который завершается, когда игрок (объект с тегом «Player») входит во внутренность триггер‑коллайдера.
/// </summary>
public class ZoneHandler : ActionHandler
{
    [Tooltip("Тег объекта, вход которого засчитывает шаг выполненным.")]
    public string targetTag = "Player";

    /// <summary>
    /// Unity вызывает метод при входе любого коллайдера в триггер‑зону.
    /// </summary>
    /// <param name="other">Коллайдер вошедшего объекта.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Уведомляем TaskManager о действии пользователя
        FireTriggered();

        // Если шаг сейчас не активен – вышли раньше времени, менеджер сам разберётся
        if (!isActiveAction) return;

        // Проверяем, что вошёл именно нужный объект (по тегу)
        if (!other.CompareTag("Player")) return;

        // Все проверки пройдены – считаем шаг успешно выполненным
        CompleteAction(withError: false);
    }

}
