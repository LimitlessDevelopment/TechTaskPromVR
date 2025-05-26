using UnityEngine;
using UnityEngine.Events;

public enum StepStatus { NotStarted, CompletedCorrect, CompletedWithError, Skipped }

public abstract class Step : MonoBehaviour
{
    public string description;           // Описание шага 
    public StepStatus status = StepStatus.NotStarted;
    public int stepIndex;                // Порядковый номер шага в сценарии 
    public delegate void StepCompleted(Step step);
    public event StepCompleted OnStepCompleted;

    public delegate void StepActivated(Step step);
    public event StepActivated OnStepActivated;
    protected bool isActiveStep = false; // Активен ли сейчас этот шаг

    // Вызывается менеджером , когда шаг становится текущим
    public virtual void ActivateStep()
    {
        isActiveStep = true;
        OnStepActivated?.Invoke(this);

    }

    // Завершение шага (вызывается из подклассов при выполнении действия)
    protected void CompleteStep(bool withError)
    {
        if (!isActiveStep)
        {
            // Если шаг не активен, но вызван Complete (непоочередное выполнение)
            // Тем не менее, пометим статус и сообщим менеджеру
        }
        isActiveStep = false;
        
        // Присваиваем статус шага (корректно или с ошибкой)
        status = withError ? StepStatus.CompletedWithError : StepStatus.CompletedCorrect;
        // событие завершения шага
        OnStepCompleted?.Invoke(this);
    }
}
