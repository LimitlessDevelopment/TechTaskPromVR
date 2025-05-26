using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenuUI : MonoBehaviour
{
    public string TrainingSceneId;
    public void StartTraining()
    {
        // Загрузка сцены тренировки
        //if(SceneManager.GetSceneAt(TrainingSceneId) != null) 
        SceneManager.LoadScene(TrainingSceneId);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
