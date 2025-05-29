using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static ActionHandler;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// TaskManager отвечает за прохождение сценария, состоящего из групп шагов.
/// </summary>
public class TaskManager : MonoBehaviour
{
    #region ДАННЫЕ СЦЕНАРИЯ
    // Группа шагов.
    [System.Serializable]
    public class Group
    {
        [Tooltip("Название группы, выводится в итоговом отчёте.")]
        public string title = "Новая группа";
        [Tooltip("Шаги сценария внутри группы.")]
        public List<ActionHandler> steps = new();
    }

    [Tooltip("Список групп сценария (заполняется в инспекторе).")]
    public List<Group> groups = new();
    #endregion

    #region UI И СОБЫТИЯ
    [Tooltip("Текстовое поле для вывода итогов выполнения.")]
    public TMP_Text resultsText;           // UI Text для вывода списка результатов

    // События, на которые могут подписаться другие системы (звук, UI и т.п.)
    public UnityEvent OnWrongAction; // пользователь совершил ошибочное действие
    public UnityEvent OnRightAction; // пользователь выполнил шаг корректно
    public UnityEvent OnGroupEnded;// завершена текущая группа шагов
    public UnityEvent OnSceneEnded;// завершён весь сценарий
    public UnityEvent OnActionStarted;  // стартовал новый шаг
    public UnityEvent OnActionEnded; // завершён текущий шаг
    public UnityEvent OnActionSkiped; // шаг был пропущен (нарушен порядок)
    public UnityEvent OnSceneTaskStarted; // сценарий начал выполнение (первый шаг активирован)
    #endregion

    #region ТЕКУЩЕЕ СОСТОЯНИЕ
    public int _currentActionIndex = 0;  // индекс шага внутри активной группы
    public int _currentGroupIndex = 0; // индекс активной группы
    public bool _scenarioCompleted = false;
    #endregion

    #region UNITY 
    void OnEnable() => ActionHandler.OnAnyTriggered += OnAnyTriggered;
    void OnDisable() => ActionHandler.OnAnyTriggered -= OnAnyTriggered;
    void Start() => InitializeGroups();
    #endregion

    #region ЛОГИКА ВЫПОЛНЕНИЯ ШАГОВ
    /// <summary>
    /// Подготавливаем все группы: сбрасываем статусы, назначаем индексы и подписываемся на события.
    /// </summary>
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
                    Debug.LogWarning("TaskManager: обнаружен пустой шаг (null).");
                    return;
                }
                step.ActionIndex = i; // убедимся, что индекс соответствует позиции в списке
                step.status = ActionStatus.NotStarted; // сброс статуса
                step.OnActionCompleted += OnActionCompleted; // подписка на событие завершения
            }
        }
    }

    /// <summary>
    /// Активировать шаг по двум индексам: группа и шаг внутри группы.
    /// </summary>
    private void ActivateStep(int groupIndex, int stepIndex)
    {
        _currentActionIndex = stepIndex;
        _currentGroupIndex = groupIndex;

        if (_currentActionIndex < groups[_currentGroupIndex].steps.Count)
        {
            ActionHandler step = groups[_currentGroupIndex].steps[_currentActionIndex];
            step.ActivateStep(); // уведомляем ActionHandler, что он стал активным

            OnActionStarted?.Invoke();  // Событие - стартовал новый шаг

            // Можно показать подсказку или уведомление о новом шаге (например, текст инструкции)
            Debug.Log("Начало шага: " + step.description);
        }
    }

    /// <summary>
    /// Обработчик события завершения шага, вызываемый ActionHandler'ом.
    /// </summary>
    private void OnActionCompleted(ActionHandler completedAction)
    {
        int actionIndex = completedAction.ActionIndex;

        if (_scenarioCompleted) return;  // если сценарий уже завершён, игнорируем события

        if (actionIndex == _currentActionIndex)
        {
            // Завершён ожидаемый текущий шаг

            // Переходим к следующему шагу
            int nextIndex = _currentActionIndex + 1;
            if (nextIndex < groups[_currentGroupIndex].steps.Count)
            {
                if (completedAction.status == ActionStatus.CompletedCorrect) OnRightAction?.Invoke();
                if (completedAction.status == ActionStatus.CompletedWithError) OnWrongAction?.Invoke();

                Debug.Log("Завершение шага: " + completedAction.description);
                OnActionEnded?.Invoke();

                ActivateStep(_currentGroupIndex, nextIndex); // В группе есть ещё шаги – активируем следующий
            }
            else
            {
                FindNextNonEmptyGroup(_currentGroupIndex + 1); // Группа закончилась – ищем следующую непустую группу
            }
        }
        else if (actionIndex > _currentActionIndex) // Выполнен шаг впереди текущего (нарушена последовательность)
        {
            OnActionSkiped?.Invoke();

            for (int skip = _currentActionIndex; skip < actionIndex; skip++) // Отметим все шаги, которые перепрыгнули, как пропущенные
            {
                groups[_currentGroupIndex].steps[skip].status = ActionStatus.Skipped;
            }
            // Переходим на шаг после него
            if (actionIndex + 1 < groups[_currentGroupIndex].steps.Count)
            {
                ActivateStep(_currentGroupIndex, actionIndex + 1); // Переходим на следующий шаг, если он есть
            }
            else
            {
                FindNextNonEmptyGroup(_currentGroupIndex + 1); // Текущая группа исчерпана – ищем следующую
            }
        }
        else
        {
            //событие от уже пройденного шага (можно игнорировать)
        }
    }

    /// <summary>
    /// Переходит к первой непустой группе, начиная с индекса nextGroupIndex.
    /// Если непустых групп больше нет – завершает сценарий.
    /// </summary>
    private void FindNextNonEmptyGroup(int nextGroupIndex)
    {
        bool found = false;
        for (int g = nextGroupIndex; g < groups.Count; g++)
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
            // Нашли следующую группу
            OnGroupEnded?.Invoke();
            ActivateStep(_currentGroupIndex, _currentActionIndex); // здесь
            Debug.Log("New groupe");
        }
        // Если дошли сюда – групп не осталось, завершаем все задания
        else EndAllActions();
    }

    /// <summary>
    /// Формирует отчёт и завершает сценарий.
    /// </summary>
    private void EndAllActions()
    {
        _scenarioCompleted = true;

        OnSceneEnded?.Invoke();

        // Формируем текст отчёта по шагам
        string resultStr = "Результаты тренировки:\n";
        foreach (var group in groups)
        {
            resultStr += "\n" + group.title + ":" + "\n";
            foreach (ActionHandler step in group.steps)
            {
                string statusStr;
                switch (step.status)
                {
                    case ActionStatus.CompletedCorrect: statusStr = "Выполнено"; break;
                    case ActionStatus.CompletedWithError: statusStr = "С ошибкой"; break;
                    case ActionStatus.Skipped: statusStr = "Пропущен"; break;
                    default: statusStr = "—"; break;
                }
                resultStr += $"- Шаг {step.ActionIndex + 1}: {step.description} — {statusStr}\n";
            }

        }

        // Выводим отчёт в UI
        resultsText.text = resultStr;
    }

    /// <summary>
    /// Глобальный обработчик любого действия от ActionHandler'ов.
    /// Используется для фиксации «промахов» (не тот объект / не в нужное время).
    /// </summary>
    private void OnAnyTriggered(ActionHandler triggered)
    {
        if (_scenarioCompleted || _currentActionIndex >= groups[_currentGroupIndex].steps.Count) return;

        var expected = groups[_currentGroupIndex].steps[_currentActionIndex];  // Текущий ожидаемый шаг

        if (triggered == expected) return; // Если нужен именно этот объект – его Handler сам завершит шаг

        // Тип совпал, но объект другой → промах
        if (triggered.GetType() == expected.GetType() && expected.status == ActionStatus.NotStarted)
        {
            Debug.Log($"Промах: ожидали '{expected.name}', а получили '{triggered.name}'");
            expected.status = ActionStatus.CompletedWithError;
            OnActionCompleted(expected);           // форсируем переход
        }
    }

    /// <summary>
    /// Запускает сценарий с самого начала (первый шаг первой непустой группы).
    /// </summary>
    public void ActivateFirstStep()
    {
        // Активируем первый шаг
        if (groups[_currentGroupIndex].steps.Count > 0)
        {
            ActivateStep(_currentGroupIndex, 0);
            OnSceneTaskStarted?.Invoke();
        }
    }
    #endregion

    #region ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ UI
    /// <summary>
    /// Перезапуск текущей сцены тренировки.
    /// </summary>
    public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    /// <summary>
    /// Выход в лобби (загрузка сцены "Lobby").
    /// </summary>
    public void ExitToLobby() => SceneManager.LoadScene("Lobby");

    /// <summary>
    /// Завершение приложения.
    /// </summary>
    public void Quit() => Application.Quit();
    #endregion
}
