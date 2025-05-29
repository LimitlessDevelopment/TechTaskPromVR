using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Включает / выключает заданные наборы объектов в зависимости от текущего индекса группы в TaskManager.
/// Каждый индекс группы «подсвечивает» (активирует) свою коллекцию объектов,а остальные делает неактивными.
/// </summary>
public class EnableObjectsOnGroupeINdex : MonoBehaviour
{
    // Структура «описание + список объектов», чтобы наглядно задавать в инспекторе.
    [System.Serializable] 
    public class ObjectsList
    {
        [Tooltip("Краткое описание списка (только для удобства в инспекторе).")]
        public string Description;
        [Tooltip("Список объектов, которые нужно активировать при выбранной группе.")]
        public List<GameObject> Objects;
    }

    [Tooltip("Массив списков объектов, соответствующих индексам групп TaskManager'а.")]
    public List<ObjectsList> objects = new ();

    [Tooltip("Ссылка на TaskManager, у которого считываем текущий индекс группы.")]
    public TaskManager manager;

    //Храним последний обработанный индекс, чтобы реагировать только на изменения.
    int _currentIndex = 1;

    // Update is called once per frame
    void Update()
    {
        if (objects.Count == 0 || manager == null) return;

        // Проверяем, изменился ли индекс группы; если нет – ничего не делаем
        if (!IntHasChanged(ref _currentIndex, manager._currentGroupIndex)) return;

        // Проходим по всем спискам и активируем только тот, чей индекс совпал
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
    /// Возвращает true, если новое значение (newIndex) отличается от текущего (currentIndex).
    /// При изменении обновляет currentIndex.
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
