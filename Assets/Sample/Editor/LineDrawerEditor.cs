using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AillieoUtils
{
    [CustomEditor(typeof(LineDrawer))]
    public class LineDrawerEditor : Editor
    {
        private LineDrawer lineDrawer;

        private Editor configEditor;

        private float lineWidth = 5f;
        private bool autoUpdate = false;

        private ReorderableList reorderableList;

        SerializedProperty config;
        SerializedProperty points1;


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            reorderableList.DoLayoutList();

            EditorGUILayout.Space();

            lineWidth = EditorGUILayout.FloatField(new GUIContent("Line Width"), lineWidth);

            autoUpdate = EditorGUILayout.Toggle(new GUIContent("Auto Update"), autoUpdate);

            if (GUILayout.Button("Do Update"))
            {
                CornerRounder.RoundCorner(lineDrawer.Points1, lineDrawer.Points2, lineDrawer.Config);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(config, new GUIContent("Config"));

            EditorGUILayout.Space();

            if(configEditor != null)
            {
                EditorGUILayout.BeginVertical("box");

                configEditor.OnInspectorGUI();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            SceneView.RepaintAll();

            if (ActiveEditorTracker.sharedTracker.isLocked)
            {
                if (GUILayout.Button("Unlock Inspector"))
                {
                    ActiveEditorTracker.sharedTracker.isLocked = false;
                    ActiveEditorTracker.sharedTracker.ForceRebuild();
                }
            }
            else
            {
                if (GUILayout.Button("Lock Inspector"))
                {
                    ActiveEditorTracker.sharedTracker.isLocked = true;
                    ActiveEditorTracker.sharedTracker.ForceRebuild();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            lineDrawer = target as LineDrawer;

            configEditor = CreateEditor(lineDrawer.Config);

            config = serializedObject.FindProperty("config");

            points1 = serializedObject.FindProperty("points1");
            reorderableList = new ReorderableList(serializedObject, points1);
            reorderableList.drawHeaderCallback += rect => GUI.Label(rect, "Point List");
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
            reorderableList.drawElementCallback += (rect, index, isActive, isFocused) => {
                var oneEntry = points1.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, oneEntry, GUIContent.none);
            };

            SceneView.onSceneGUIDelegate += OnSceneUpdate;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneUpdate;
        }

        private void OnSceneUpdate(SceneView sceneView)
        {
            if(lineDrawer.Points1 == null || lineDrawer.Points2 == null || lineDrawer.Config == null)
            {
                return;
            }

            if (autoUpdate)
            {
                CornerRounder.RoundCorner(lineDrawer.Points1, lineDrawer.Points2, lineDrawer.Config);
            }

            DrawLine(lineDrawer.Points1, Color.blue);
            DrawLine(lineDrawer.Points2.ToArray(), Color.red);

        }

        private void DrawLine(Vector3[] points, Color color)
        {
            Color backUp = Handles.color;
            Handles.color = color;
            Handles.DrawAAPolyLine(lineWidth, points);
            Handles.color = backUp;


        }

    }

}
