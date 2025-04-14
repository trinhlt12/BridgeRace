using UnityEditor;
using UnityEngine;

namespace _GAME.Scripts.FSM.Bridge.Editor
{
    [CustomEditor(typeof(BridgeBuilder))]
    public class BridgeBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var bridgeBuilder = (BridgeBuilder)target;

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Build Bridge in Editor"))
            {
                bridgeBuilder.RebuildInEditor();
                EditorUtility.SetDirty(bridgeBuilder);
            }
        }
    }
}