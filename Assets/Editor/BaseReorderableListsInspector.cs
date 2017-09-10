using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
public abstract class BaseReorderableListsInspector : UnityEditor.Editor {

    public override void OnInspectorGUI() {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();


        bool checkChildren = true;
        while (prop.NextVisible(checkChildren)) {
            if (prop.isArray) {
                ReorderableListGUI.Title(prop.displayName);
                if (serializedObject.isEditingMultipleObjects) {
                        GUILayout.Label("--- step array not editable in multi selection mode ---");
                } else {
					ReorderableListGUI.ListField(prop, 200);
					ReorderableListGUI.CalculateListFieldHeight (prop);
                    checkChildren = false;
                }
            } else {
                EditorGUILayout.PropertyField(prop);
                checkChildren = true;
            }
        }
			
        serializedObject.ApplyModifiedProperties();

        drawCustomUI();
    }

    protected abstract void drawCustomUI();
}
