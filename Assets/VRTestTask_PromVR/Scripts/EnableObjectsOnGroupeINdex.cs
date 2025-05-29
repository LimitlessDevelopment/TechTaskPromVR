using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������� / ��������� �������� ������ �������� � ����������� �� �������� ������� ������ � TaskManager.
/// ������ ������ ������ ������������� (����������) ���� ��������� ��������,� ��������� ������ �����������.
/// </summary>
public class EnableObjectsOnGroupeINdex : MonoBehaviour
{
    // ��������� ��������� + ������ ��������, ����� �������� �������� � ����������.
    [System.Serializable] 
    public class ObjectsList
    {
        [Tooltip("������� �������� ������ (������ ��� �������� � ����������).")]
        public string Description;
        [Tooltip("������ ��������, ������� ����� ������������ ��� ��������� ������.")]
        public List<GameObject> Objects;
    }

    [Tooltip("������ ������� ��������, ��������������� �������� ����� TaskManager'�.")]
    public List<ObjectsList> objects = new ();

    [Tooltip("������ �� TaskManager, � �������� ��������� ������� ������ ������.")]
    public TaskManager manager;

    //������ ��������� ������������ ������, ����� ����������� ������ �� ���������.
    int _currentIndex = 1;

    // Update is called once per frame
    void Update()
    {
        if (objects.Count == 0 || manager == null) return;

        // ���������, ��������� �� ������ ������; ���� ��� � ������ �� ������
        if (!IntHasChanged(ref _currentIndex, manager._currentGroupIndex)) return;

        // �������� �� ���� ������� � ���������� ������ ���, ��� ������ ������
        for (int i = 0; i < objects.Count; i++)
        {
            if (i == _currentIndex)
            { 
                foreach (var obj in objects[i].Objects) obj.SetActive(true);
            }
            else
            {
                foreach (var obj in objects[i].Objects) obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ���������� true, ���� ����� �������� (newIndex) ���������� �� �������� (currentIndex).
    /// ��� ��������� ��������� currentIndex.
    /// </summary>
    bool IntHasChanged(ref int currentIndex, int newIndex)
    {
        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            return true;
        }
            return false;
    }
}
