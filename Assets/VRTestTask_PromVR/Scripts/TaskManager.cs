using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using static ActionHandler;

public class TaskManager : MonoBehaviour
{
    public List<ActionHandler> steps;            // Список шагов в порядке выполнения
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip errorClip;
    public Canvas resultsCanvas;        // Canvas с итоговым экраном (изначально отключен)
    public Text resultsText;           // UI Text для вывода списка результатов

    private int currentActionIndex = 0;
    private bool scenarioCompleted = false;

    void Start()
    {
        // Подписываемся на события всех шагов и сбрасываем статусы
        for (int i = 0; i < steps.Count; i++)
        {
            ActionHandler step = steps[i];
            step.ActionIndex = i; // убедимся, что индекс соответствует позиции в списке
            step.status = ActionStatus.NotStarted;
            step.OnActionCompleted += OnActionCompleted;
        }
    }

    private void OnActionCompleted(ActionHandler completedAction)
    {
        
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
}
