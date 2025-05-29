#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Кастомный инспектор для TaskManager>.
/// Позволяет удобно редактировать Список групп сценария (ReorderableList).  
/// </summary>
[CustomEditor(typeof(TaskManager))]
public class TaskManagerEditor : Editor
{

    private ReorderableList _groupList;                               // внешний список групп
    private readonly System.Collections.Generic.Dictionary<int, ReorderableList> _stepLists = new(); // кэш вложенных списков

    private SerializedProperty resultsText;
    private SerializedProperty _currentActionIndex;
    private SerializedProperty _currentGroupIndex;
    private SerializedProperty _scenarioCompleted;

    //SerializedProperty ссылки на UnityEvents
    private SerializedProperty _onWrong;
    private SerializedProperty _onRight;
    private SerializedProperty _onGroupEnded;
    private SerializedProperty _onSceneEnded;
    private SerializedProperty _onActionStarted;
    private SerializedProperty _onActionEnded;
    private SerializedProperty _onActionSkipped;

    #region ИНИЦИАЛИЗАЦИЯ EDITOR’A

    private void OnEnable()
    {
        // Cписок групп
        SerializedProperty groupsProp = serializedObject.FindProperty("groups");

        _groupList = new ReorderableList(serializedObject, groupsProp, draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Группы сценария"),

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var groupProp = groupsProp.GetArrayElementAtIndex(index);
                var titleProp = groupProp.FindPropertyRelative("title");
                EditorGUI.PropertyField(rect, titleProp, GUIContent.none);
            },

            onAddCallback = list =>
            {
                groupsProp.InsertArrayElementAtIndex(groupsProp.arraySize);
                var newGroup = groupsProp.GetArrayElementAtIndex(groupsProp.arraySize - 1);
                newGroup.FindPropertyRelative("title").stringValue = "Новая группа";
                newGroup.FindPropertyRelative("steps").ClearArray();
            }
        };
        // UI 
        resultsText = serializedObject.FindProperty("resultsText");

        //Debug
        _currentActionIndex = serializedObject.FindProperty("_currentActionIndex");
        _currentGroupIndex = serializedObject.FindProperty("_currentGroupIndex");
        _scenarioCompleted = serializedObject.FindProperty("_scenarioCompleted");

        //ссылки на UnityEvent
        _onWrong = serializedObject.FindProperty("OnWrongAction");
        _onRight = serializedObject.FindProperty("OnRightAction");
        _onGroupEnded = serializedObject.FindProperty("OnGroupEnded");
        _onSceneEnded = serializedObject.FindProperty("OnSceneEnded");
        _onActionStarted = serializedObject.FindProperty("OnActionStarted");
        _onActionEnded = serializedObject.FindProperty("OnActionEnded");
        _onActionSkipped = serializedObject.FindProperty("OnActionSkiped");
    }

    #endregion

    #region ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

    /// <summary>
    /// Возвращает подсписок шагов для указанной группы.
    /// </summary>
    private ReorderableList GetStepList(int groupIdx)
    {
        if (_stepLists.TryGetValue(groupIdx, out var list)) return list;

        var groupsProp = serializedObject.FindProperty("groups");
        var stepsProp = groupsProp.GetArrayElementAtIndex(groupIdx).FindPropertyRelative("steps");

        list = new ReorderableList(serializedObject, stepsProp, true, true, true, true)
        {
            drawHeaderCallback = r => EditorGUI.LabelField(r, "Шаги группы"),
            elementHeight = 38f,

            drawElementCallback = (rect, i, a, f) =>
            {
                var element = stepsProp.GetArrayElementAtIndex(i);
                var handler = element.objectReferenceValue as ActionHandler;

                string desc = handler ? handler.description : "<пусто>";
                string type = handler ? handler.GetType().Name : "None";

                Rect line1 = new Rect(rect.x, rect.y + 2, rect.width, 16);
                Rect line2 = new Rect(rect.x, rect.y + 18, rect.width, 16);
                EditorGUI.LabelField(line1, $"{i + 1}. {desc}");
                EditorGUI.LabelField(line2, $"Тип: {type}");

                // ObjectField справа – позволяет перетащить другой объект в ячейку
                Rect obj = new Rect(rect.x + rect.width - 90, rect.y + 2, 90, 16);
                EditorGUI.PropertyField(obj, element, GUIContent.none);
            },

            onAddDropdownCallback = (btnRect, l) => ShowAddStepMenu(stepsProp)
        };

        _stepLists[groupIdx] = list;
        return list;
    }

    /// <summary>
    /// Отображает выпадающее меню со всеми ActionHandler’ами сцены.
    /// Выбранный элемент добавляется в список шагов.
    /// </summary>
    private void ShowAddStepMenu(SerializedProperty stepsProp)
    {
        GenericMenu menu = new GenericMenu();
        var allHandlers = Object.FindObjectsByType<ActionHandler>(FindObjectsSortMode.None);

        if (allHandlers.Length == 0)
        {
            menu.AddDisabledItem(new GUIContent("В сцене нет ActionHandler"));
        }
        else
        {
            foreach (var h in allHandlers)
            {
                string path = $"{h.GetType().Name}/{h.name}";
                menu.AddItem(new GUIContent(path), false, () =>
                {
                    stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
                    stepsProp.GetArrayElementAtIndex(stepsProp.arraySize - 1).objectReferenceValue = h;
                    serializedObject.ApplyModifiedProperties();
                });
            }
        }
        menu.ShowAsContext();
    }

    #endregion

    #region GUI
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // список групп
        _groupList.DoLayoutList();

        // список шагов выбранной группы
        if (_groupList.index >= 0 && _groupList.index < _groupList.count)
        {
            EditorGUILayout.Space(6);
            GetStepList(_groupList.index).DoLayoutList();
        }

        // UnityEvent
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("События", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_onWrong);
        EditorGUILayout.PropertyField(_onRight);
        EditorGUILayout.PropertyField(_onGroupEnded);
        EditorGUILayout.PropertyField(_onSceneEnded);
        EditorGUILayout.PropertyField(_onActionStarted);
        EditorGUILayout.PropertyField(_onActionEnded);
        EditorGUILayout.PropertyField(_onActionSkipped);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(resultsText);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_currentGroupIndex);
        EditorGUILayout.PropertyField(_currentActionIndex);
        EditorGUILayout.PropertyField(_scenarioCompleted);
        serializedObject.ApplyModifiedProperties();
    }

    #endregion
}
#endif
