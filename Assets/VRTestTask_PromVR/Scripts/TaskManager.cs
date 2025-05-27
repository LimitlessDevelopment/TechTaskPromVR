using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using static ActionHandler;
using UnityEngine.Audio;
using TMPro;


public class TaskManager : MonoBehaviour
{
    public List<ActionHandler> steps;            // Список шагов в порядке выполнения
    //public AudioSource audioSource;
    //public AudioClip successClip;
    //public AudioClip errorClip;
    public Canvas resultsCanvas;        // Canvas с итоговым экраном (изначально отключен)
    public TMP_Text resultsText;           // UI Text для вывода списка результатов

    private int currentActionIndex = 0;
    private bool scenarioCompleted = false;

    void Start()
    {
        // Подписываемся на события всех шагов и сбрасываем статусы
        for (int i = 0; i < steps.Count; i++)
        {
            ActionHandler step = steps[i];
            if(step == null)
            {
                Debug.LogWarning("Action Not found!");
                return;
            } 
            step.ActionIndex = i; // убедимся, что индекс соответствует позиции в списке
            step.status = ActionStatus.NotStarted;
            step.OnActionCompleted += OnActionCompleted;
        }
        // Активируем первый шаг
        if (steps.Count > 0)
        {
            ActivateStep(0);
        }
    }
    private void ActivateStep(int index)
    {
        currentActionIndex = index;
        if (currentActionIndex < steps.Count)
        {
            ActionHandler step = steps[currentActionIndex];
            step.ActivateStep();
            // Можно показать подсказку или уведомление о новом шаге (например, текст инструкции)
            Debug.Log("Начало шага: " + step.description);
        }
    }
    private void OnActionCompleted(ActionHandler completedAction)
    {
        int idx = completedAction.ActionIndex;
       // bool wasError = (completedAction.status == ActionStatus.CompletedWithError);
        if (scenarioCompleted) return;  // если сценарий уже завершён, игнорируем события

        if (idx == currentActionIndex)
        {
            // Завершён ожидаемый текущий шаг

            // Переходим к следующему шагу
            int nextIndex = currentActionIndex + 1;
            if (nextIndex < steps.Count)
            {
                Debug.Log("Завершение шага: " + completedAction.description);
                ActivateStep(nextIndex);
            }
            else
            {
                EndAllActions();
            }
        }
        else if (idx > currentActionIndex)
        {
            // Выполнен шаг впереди текущего (нарушена последовательность)
            // Отметим все шаги, которые перепрыгнули, как пропущенные
            for (int skip = currentActionIndex; skip < idx; skip++)
            {
                steps[skip].status = ActionStatus.Skipped;
            }
            // Переходим на шаг после него
            if (idx + 1 < steps.Count)
            {
                ActivateStep(idx + 1);
            }
            else
            {
                EndAllActions();
            }
        }
        else
        {
            // idx < currentStepIndex: событие от уже пройденного шага (можно игнорировать)
        }
    }
    private void EndAllActions()
    {
        scenarioCompleted = true;
        // Формируем текст отчёта по шагам
        string resultStr = "Результаты тренировки:\n";
        foreach (ActionHandler step in steps)
        {
            string statusStr;
            switch (step.status)
            {
                case ActionStatus.CompletedCorrect: statusStr = "✔️ Выполнено"; break;
                case ActionStatus.CompletedWithError: statusStr = "❌ С ошибкой"; break;
                case ActionStatus.Skipped: statusStr = "⚠️ Пропущен"; break;
                default: statusStr = "—"; break;
            }
            resultStr += $"- Шаг {step.ActionIndex+ 1}: {step.description} — {statusStr}\n";
        }
        resultsText.text = resultStr;
        resultsCanvas.enabled = true;
        // Сохранение результатов (пример: количество ошибок в PlayerPrefs)

    }
    public void RestartScene()
    {
        // Перезапуск сцены тренировки
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToLobby()
    {
        // Возврат в лобби
        SceneManager.LoadScene("Lobby");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
