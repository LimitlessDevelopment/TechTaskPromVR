using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Простое меню в лобби, содержащее две кнопки: «Начать тренировку» и «Выход».
/// </summary>
public class LobbyMenuUI : MonoBehaviour
{
    /// имя сцены тренировки, которую нужно загрузить.
    public string TrainingScene;
    /// <summary>
    /// Вызывается кнопкой меню: загружает сцену тренировки.
    /// </summary>
    public void StartTraining()
    {
        // Проверяем, указана ли сцена
        if (string.IsNullOrEmpty(TrainingScene))
        {
            Debug.LogError("LobbyMenuUI: TrainingSceneId не задан!");
            return;
        }

        // Загружаем указанную сцену.
        SceneManager.LoadScene(TrainingScene);
    }
    /// <summary>
    /// Вызывается кнопкой меню: завершает приложение.
    /// </summary>
    public void Quit()
    {
        Debug.Log("LobbyMenuUI: Выход из приложения.");
        Application.Quit();
    }
}
