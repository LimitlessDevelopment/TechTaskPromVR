#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TaskManager))]
public class TaskManagerEditor : Editor
{
    private ReorderableList groupList;
    private readonly System.Collections.Generic.Dictionary<int, ReorderableList> stepLists = new();

    void OnEnable()
    {
        SerializedProperty groupsProp = serializedObject.FindProperty("groups");

        /* —––––––––  Список групп  –––––––– */
        groupList = new ReorderableList(serializedObject, groupsProp, true, true, true, true);

        groupList.drawHeaderCallback =
            r => EditorGUI.LabelField(r, "Группы сценария");

        groupList.drawElementCallback = (r, i, act, foc) =>
        {
            var g = groupsProp.GetArrayElementAtIndex(i);
            var title = g.FindPropertyRelative("title");
            EditorGUI.PropertyField(r, title, GUIContent.none);
        };

        groupList.onAddCallback = l =>
        {
            groupsProp.InsertArrayElementAtIndex(groupsProp.arraySize);
            var g = groupsProp.GetArrayElementAtIndex(groupsProp.arraySize - 1);
            g.FindPropertyRelative("title").stringValue = "Новая группа";
            g.FindPropertyRelative("steps").ClearArray();
        };
    }

    /* —––––––––  Вложенный список шагов  –––––––– */
    private ReorderableList GetStepList(int groupIdx)
    {
        if (stepLists.TryGetValue(groupIdx, out var rl)) return rl;

        var groupsProp = serializedObject.FindProperty("groups");
        var stepsProp = groupsProp.GetArrayElementAtIndex(groupIdx).FindPropertyRelative("steps");

        rl = new ReorderableList(serializedObject, stepsProp, true, true, true, true);

        rl.drawHeaderCallback = r => EditorGUI.LabelField(r, "Шаги группы");
        rl.elementHeight = 38f;

        rl.drawElementCallback = (r, i, act, foc) =>
        {
            var elem = stepsProp.GetArrayElementAtIndex(i);
            var handler = elem.objectReferenceValue as ActionHandler;

            string desc = handler ? handler.description : "<пусто>";
            string type = handler ? handler.GetType().Name : "None";

            Rect l1 = new Rect(r.x, r.y + 2, r.width, 16);
            Rect l2 = new Rect(r.x, r.y + 18, r.width, 16);

            EditorGUI.LabelField(l1, $"{i + 1}. {desc}");
            EditorGUI.LabelField(l2, $"Тип: {type}");

            Rect obj = new Rect(r.x + r.width - 90, r.y + 2, 90, 16);
            EditorGUI.PropertyField(obj, elem, GUIContent.none);
        };

        rl.onAddDropdownCallback = (Rect btn, ReorderableList l) =>
        {
            GenericMenu m = new GenericMenu();
            var all = Object.FindObjectsByType<ActionHandler>(FindObjectsSortMode.None);

            if (all.Length == 0)
                m.AddDisabledItem(new GUIContent("В сцене нет ActionHandler"));
            else
            {
                foreach (var h in all)
                {
                    string path = $"{h.GetType().Name}/{h.name}";
                    m.AddItem(new GUIContent(path), false, () =>
                    {
                        stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
                        stepsProp.GetArrayElementAtIndex(stepsProp.arraySize - 1).objectReferenceValue = h;
                        serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            m.ShowAsContext();
        };

        stepLists[groupIdx] = rl;
        return rl;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        groupList.DoLayoutList();

        if (groupList.index >= 0 && groupList.index < groupList.count)
        {
            EditorGUILayout.Space(6);
            GetStepList(groupList.index).DoLayoutList();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
