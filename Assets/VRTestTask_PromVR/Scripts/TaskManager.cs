using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using static ActionHandler;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Events;


public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class Group
    {
        public string title = "Новая группа";
        public List<ActionHandler> steps = new();
    }

    [Header("Сценарий")]
    public List<Group> groups = new();          // ↔ заполняете в инспекторе

    public TMP_Text resultsText;           // UI Text для вывода списка результатов

    public UnityEvent OnWrongAction;
    public UnityEvent OnRightAction;
    public UnityEvent OnGroupEnded;
    public UnityEvent OnSceneEnded;
    public UnityEvent OnActionStarted;
    public UnityEvent OnActionEnded;
    public UnityEvent OnActionSkiped;

    private int _currentActionIndex = 0;
    private int _currentGroupIndex = 0;
    private bool _scenarioCompleted = false;

    void OnEnable() => ActionHandler.OnAnyTriggered += OnAnyTriggered;
    void OnDisable() => ActionHandler.OnAnyTriggered -= OnAnyTriggered;

    void Start() => InitializeGroups();


    private void InitializeGroups()
    {
        // Подписываемся на события всех шагов и сбрасываем статусы
        foreach (var group in groups)
        {
            for (int i = 0; i < group.steps.Count; i++)
            {
                ActionHandler step = group.steps[i];
                if (step == null)
                {
                    Debug.LogWarning("Action Not found!");
                    return;
                }
                step.ActionIndex = i; // убедимся, что индекс соответствует позиции в списке
                step.status = ActionStatus.NotStarted;
                step.OnActionCompleted += OnActionCompleted;
            }
        }
    }

    private void ActivateStep(int groupIndex, int stepIndex)
    {
        _currentActionIndex = stepIndex;
        _currentGroupIndex = groupIndex;

        if (_currentActionIndex < groups[_currentGroupIndex].steps.Count)
        {
            ActionHandler step = groups[_currentGroupIndex].steps[_currentActionIndex];
            step.ActivateStep();
            OnActionStarted?.Invoke();
            // Можно показать подсказку или уведомление о новом шаге (например, текст инструкции)
            Debug.Log("Начало шага: " + step.description);
        }
    }
    private void OnActionCompleted(ActionHandler completedAction)
    {
        int actionIndex = completedAction.ActionIndex;
        // bool wasError = (completedAction.status == ActionStatus.CompletedWithError);

        if (_scenarioCompleted) return;  // если сценарий уже завершён, игнорируем события

        if (actionIndex == _currentActionIndex)
        {
            // Завершён ожидаемый текущий шаг

            // Переходим к следующему шагу
            int nextIndex = _currentActionIndex + 1;
            if (nextIndex < groups[_currentGroupIndex].steps.Count)
            {
                if(completedAction.status == ActionStatus.CompletedCorrect) OnRightAction?.Invoke();
                if(completedAction.status == ActionStatus.CompletedWithError) OnWrongAction?.Invoke();

                Debug.Log("Завершение шага: " + completedAction.description);
                OnActionEnded?.Invoke();
                ActivateStep(_currentGroupIndex, nextIndex);
            }
            else
            {
                FindNextNonEmptyGroup(_currentGroupIndex + 1);
            }
        }
        else if (actionIndex > _currentActionIndex)
        {
            // Выполнен шаг впереди текущего (нарушена последовательность)
            OnActionSkiped?.Invoke();
            // Отметим все шаги, которые перепрыгнули, как пропущенные

            for (int skip = _currentActionIndex; skip < actionIndex; skip++)
            {
                groups[_currentGroupIndex].steps[skip].status = ActionStatus.Skipped;
            }
            // Переходим на шаг после него
            if (actionIndex + 1 < groups[_currentGroupIndex].steps.Count)
            {
                ActivateStep(_currentGroupIndex, actionIndex + 1);
            }
            else
            {
                FindNextNonEmptyGroup(_currentGroupIndex + 1);
            }
        }
        else
        {
            //событие от уже пройденного шага (можно игнорировать)
        }
    }

    private void FindNextNonEmptyGroup(int startG)
    {
        bool found = false;
        for (int g = startG; g < groups.Count; g++)
        {
            if (groups[g].steps.Count > 0)
            {
                _currentGroupIndex = g;
                _currentActionIndex = 0;
                found = true;
            }
        }

        if (found)
        {
            OnGroupEnded?.Invoke(); 
            ActivateStep(_currentGroupIndex, _currentActionIndex); // здесь
            Debug.Log("New groupe");
        }

        else EndAllActions();
    }

    private void OnAnyTriggered(ActionHandler triggered)
    {
        if (_scenarioCompleted || _currentActionIndex >= groups[_currentGroupIndex].steps.Count) return;

        var expected = groups[_currentGroupIndex].steps[_currentActionIndex];

        // Если нужен именно этот объект – его Handler сам завершит шаг
        if (triggered == expected) return;

        // Тип совпал, но объект другой → промах
        if (triggered.GetType() == expected.GetType() &&
            expected.status == ActionStatus.NotStarted)
        {
            Debug.Log($"Промах: ожидали '{expected.name}', а получили '{triggered.name}'");
            expected.status = ActionStatus.CompletedWithError;
            OnActionCompleted(expected);           // форсируем переход
        }

        // Для действия иного типа решайте сами (обычно игнор)
    }
    private void EndAllActions()
    {
        _scenarioCompleted = true;

        OnSceneEnded?.Invoke(); 
        // Формируем текст отчёта по шагам
        string resultStr = "Результаты тренировки:\n";
        foreach (var group in groups)
        {
            resultStr += "\n" + group.title + ":";
            foreach (ActionHandler step in group.steps)
            {
                string statusStr;
                switch (step.status)
                {
                    case ActionStatus.CompletedCorrect: statusStr = "✔️ Выполнено"; break;
                    case ActionStatus.CompletedWithError: statusStr = "❌ С ошибкой"; break;
                    case ActionStatus.Skipped: statusStr = "⚠️ Пропущен"; break;
                    default: statusStr = "—"; break;
                }
                resultStr += $"- Шаг {step.ActionIndex + 1}: {step.description} — {statusStr}\n";
            }

        }
        resultsText.text = resultStr;
        // Сохранение результатов (пример: количество ошибок в PlayerPrefs)

    }

    public void ActivateFirstStep()
    {
        // Активируем первый шаг
        if (groups[_currentGroupIndex].steps.Count > 0)
        {
            ActivateStep(_currentGroupIndex, 0);
        }
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
