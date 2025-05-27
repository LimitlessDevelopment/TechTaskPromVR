using UnityEngine;
using UnityEngine.Events;

public enum ActionStatus { NotStarted, CompletedCorrect, CompletedWithError, Skipped }

public abstract class ActionHandler : MonoBehaviour
{
    public string description;           // Описание шага 
    public ActionStatus status = ActionStatus.NotStarted;
    public int ActionIndex;                // Порядковый номер шага в сценарии 
    public delegate void ActionCompleted(ActionHandler action);
    public event ActionCompleted OnActionCompleted;

    public delegate void ActionActivated(ActionHandler action);
    public event ActionActivated OnActionActivated;
    protected bool isActiveAction = false; // Активен ли сейчас этот шаг

    // Вызывается менеджером , когда шаг становится текущим
    public virtual void ActivateStep()
    {
        isActiveAction = true;
        OnActionActivated?.Invoke(this);

    }

    // Завершение шага (вызывается из подклассов при выполнении действия)
    protected void CompleteAction(bool withError)
    {
        if (!isActiveAction)
        {
            // Если шаг не активен, но вызван Complete (непоочередное выполнение)
            // Тем не менее, пометим статус и сообщим менеджеру
        }
        isActiveAction = false;
        
        // Присваиваем статус шага (корректно или с ошибкой)
        status = withError ? ActionStatus.CompletedWithError : ActionStatus.CompletedCorrect;
        Debug.Log(status);
        // событие завершения шага
        OnActionCompleted?.Invoke(this);
    }
}
