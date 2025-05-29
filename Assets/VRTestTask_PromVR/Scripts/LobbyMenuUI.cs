using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ������� ���� � �����, ���������� ��� ������: ������� ���������� � ������.
/// </summary>
public class LobbyMenuUI : MonoBehaviour
{
    /// ��� ����� ����������, ������� ����� ���������.
    public string TrainingScene;
    /// <summary>
    /// ���������� ������� ����: ��������� ����� ����������.
    /// </summary>
    public void StartTraining()
    {
        // ���������, ������� �� �����
        if (string.IsNullOrEmpty(TrainingScene))
        {
            Debug.LogError("LobbyMenuUI: TrainingSceneId �� �����!");
            return;
        }

        // ��������� ��������� �����.
        SceneManager.LoadScene(TrainingScene);
    }
    /// <summary>
    /// ���������� ������� ����: ��������� ����������.
    /// </summary>
    public void Quit()
    {
        Debug.Log("LobbyMenuUI: ����� �� ����������.");
        Application.Quit();
    }
}
