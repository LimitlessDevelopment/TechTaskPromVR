using UnityEngine;
using UnityEngine.Events;

public enum ActionStatus { NotStarted, CompletedCorrect, CompletedWithError, Skipped }

/// <summary>
/// Базовый абстрактный класс для любого «шага»/действия тренировки.
/// Наследники (ZoneHandler, GrabHandler, ButtonHandler и т.д.) реализуют конкретный
/// способ проверки, что пользователь выполнил требуемое действие.
/// </summary>
public abstract class ActionHandler : MonoBehaviour
{
    #region ПОЛЯ, СВОЙСТВА, СОБЫТИЯ

    [Header("Описание шага")]
    [Tooltip("Текст, отображаемый в UI/отчёте.")]
    public string description;

    [Tooltip("Текущий статус выполнения этого шага.")]
    public ActionStatus status = ActionStatus.NotStarted;

    [Tooltip(" Порядковый индекс шага внутри группы (заполняет TaskManager).")]
    public int ActionIndex;

    /// <summary> Событие: этот шаг успешно завершён. </summary>
    public delegate void ActionCompleted(ActionHandler action);
    public event ActionCompleted OnActionCompleted;

    /// <summary> Событие: этот шаг активирован и теперь ожидает ввода пользователя. </summary>
    public delegate void ActionActivated(ActionHandler action);
    public event ActionActivated OnActionActivated;

    /// <summary> Статическое событие: любое ActionHandler в сцене зафиксировал действие пользователя. </summary>
    public delegate void AnyTriggered(ActionHandler action);
    public static event AnyTriggered OnAnyTriggered;

    /// <summary>
    /// Флаг: true, если именно этот шаг сейчас активен (выставляет TaskManager).
    /// Используется наследниками для проверки «правильное ли время».
    /// </summary>
    protected bool isActiveAction = false; // Активен ли сейчас этот шаг

    #endregion

    /// <summary>
    /// Вызывается TaskManager, когда шаг становится текущим.
    /// </summary>
    public virtual void ActivateStep()
    {
        isActiveAction = true;
        OnActionActivated?.Invoke(this);

    }

    /// <summary>
    /// Сообщить TaskManagerʼу, что «что‑то» прроизошло (взяли объект, нажали кнопку и т.д.).
    /// Менеджер решит: это ожидаемый объект или нет.
    /// </summary>
    protected void FireTriggered() => OnAnyTriggered?.Invoke(this);

    /// <summary>
    /// Завершить шаг. Наследники вызывают метод, когда удостоверились, что действие
    /// выполнено (или выполнено с ошибкой).
    /// </summary>
    /// <param name="withError">true — действие завершено с ошибкой (неверное), false — успешно.</param>
    protected void CompleteAction(bool withError)
    {
        // Если шаг не активен, это означает нарушение порядка, но статус всё равно фиксируем.
        if (!isActiveAction)
        {
            Debug.LogWarning($"{name}: CompleteAction вызван, хотя шаг не активен.");
        }

        isActiveAction = false;

        // Присваиваем статус шага (корректно или с ошибкой)
        status = withError ? ActionStatus.CompletedWithError : ActionStatus.CompletedCorrect;

        // событие завершения шага
        OnActionCompleted?.Invoke(this);
    }
}
