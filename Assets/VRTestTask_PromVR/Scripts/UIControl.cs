using UnityEngine;
using TMPro;

/// <summary>
/// Простой помощник для вывода текстовых уведомлений о ходе сценария.
/// </summary>
public class UIControl : MonoBehaviour
{
    /// <summary>
    /// Ссылка на компонент TextMeshPro, куда будем писать сообщения.
    /// Берётся автоматически из объекта, на котором висит скрипт.
    /// </summary>
    TMP_Text _text;

    private void Awake()
    {
        // Кешируем компонент текста, чтобы не искать его каждый раз.
        _text = GetComponent<TMP_Text>();
        if (_text == null)
            Debug.LogError("UIControl: На объекте нет TMP_Text, надписи выводиться не будут!");
    }

    /// <summary> Сцена стартовала – очищаем текст. </summary>
    public void SceneStarted() => SetText("");

    /// <summary> Пользователь совершил неверное действие. </summary>
    public void WrongAction() => SetText("Шаг выполнен неверно");

    /// <summary> Пользователь правильно выполнил шаг. </summary>
    public void RightAction() => SetText("Шаг выполнен верно");

    /// <summary> Завершён весь сценарий. </summary>
    public void SceneEnded() => SetText("Задание завершено");

    /// <summary> Завершена текущая группа шагов. </summary>
    public void GroupeEnded() => SetText("Все задания в группе завершены");

    /// <summary> Начат новый шаг. </summary>
    public void ActionStarted() => SetText("Начало нового шага");

    /// <summary> Шаг завершён. </summary>
    public void ActionEnded() => SetText("Завершение шага");

    /// <summary> Шаг пропущен из‑за нарушения порядка. </summary>
    public void ActionSkipped() => SetText("Шаг пропущен");

    /// <summary>
    /// Безопасно меняет текст, если компонент найден.
    /// </summary>
    /// <param name="msg">Строка сообщения, которую надо вывести.</param>
    private void SetText(string msg)
    {
        if (_text != null)
            _text.text = msg;
    }
}
