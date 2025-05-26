using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public List<Step> steps;            // Список шагов в порядке выполнения
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip errorClip;
    public Canvas resultsCanvas;        // Canvas с итоговым экраном (изначально отключен)
    public Text resultsText;           // UI Text для вывода списка результатов

    private int currentStepIndex = 0;
    private bool scenarioCompleted = false;

    void Start()
    {
        // Подписываемся на события всех шагов и сбрасываем статусы
        for (int i = 0; i < steps.Count; i++)
        {
            Step step = steps[i];
            step.stepIndex = i; // убедимся, что индекс соответствует позиции в списке
            step.status = StepStatus.NotStarted;
            step.OnStepCompleted+=OnStepCompleted;
        }
    }

    private void OnStepCompleted(Step completedStep)
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
