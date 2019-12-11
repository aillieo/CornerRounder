using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static AillieoUtils.CornerRounderConfig;

namespace AillieoUtils
{

    [CustomEditor(typeof(CornerRounderConfig))]
    public class CornerRounderConfigEditor : Editor
    {
        CornerRounderConfig config;

        SerializedProperty isLoop;

        SerializedProperty acuteAngleStrategy;
        SerializedProperty acuteAngleThreshold;

        SerializedProperty closePointMergeStrategy;
        SerializedProperty mergeThreshold;

        SerializedProperty radiusStrategy;
        SerializedProperty radiusTarget;
        SerializedProperty radiusMin;
        SerializedProperty radiusMax;

        SerializedProperty cornerResolutionStrategy;
        SerializedProperty cornerResolution;

        private void OnEnable()
        {
            config = target as CornerRounderConfig;

            isLoop = serializedObject.FindProperty("isLoop");

            acuteAngleStrategy = serializedObject.FindProperty("acuteAngleStrategy");
            acuteAngleThreshold = serializedObject.FindProperty("acuteAngleThreshold");

            closePointMergeStrategy = serializedObject.FindProperty("closePointMergeStrategy");
            mergeThreshold = serializedObject.FindProperty("mergeThreshold");

            radiusStrategy = serializedObject.FindProperty("radiusStrategy");
            radiusTarget = serializedObject.FindProperty("radiusTarget");
            radiusMin = serializedObject.FindProperty("radiusMin");
            radiusMax = serializedObject.FindProperty("radiusMax");

            cornerResolutionStrategy = serializedObject.FindProperty("cornerResolutionStrategy");
            cornerResolution = serializedObject.FindProperty("cornerResolution");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            // loop or not
            EditorGUILayout.PropertyField(isLoop, new GUIContent("Is Loop"));

            EditorGUILayout.Space();

            // acute angle
            EditorGUILayout.PropertyField(acuteAngleStrategy, new GUIContent("Acute Angle Strategy"));
            if (config.acuteAngleStrategy == AcuteAngleStrategy.Remove)
            {
                EditorGUILayout.PropertyField(acuteAngleThreshold, new GUIContent("Acute Angle Threshold"));
            }

            EditorGUILayout.Space();

            // merge points
            EditorGUILayout.PropertyField(closePointMergeStrategy, new GUIContent("Point Merge Strategy"));
            if(config.closePointMergeStrategy != PointMergeStrategy.Never)
            {
                EditorGUILayout.PropertyField(mergeThreshold, new GUIContent("Merge Threshold"));
            }

            EditorGUILayout.Space();

            // gen arc
            EditorGUILayout.PropertyField(radiusStrategy, new GUIContent("Radius Strategy"));
            if(config.radiusStrategy == RadiusStrategy.Unified)
            {
                EditorGUILayout.PropertyField(radiusTarget, new GUIContent("Radius"));
            }
            else if (config.radiusStrategy == RadiusStrategy.Adaptive)
            {
                EditorGUILayout.PropertyField(radiusMin, new GUIContent("Radius Min"));
                EditorGUILayout.PropertyField(radiusMax, new GUIContent("Radius Max"));
            }

            // gen points
            if (config.radiusStrategy != RadiusStrategy.Never)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(cornerResolutionStrategy, new GUIContent("CornerInfo Resolution Strategy"));

                if (config.cornerResolutionStrategy == ResolutionStrategy.ByAngle)
                {
                    EditorGUILayout.PropertyField(cornerResolution, new GUIContent("Corner Resolution"));
                }
                else if (config.cornerResolutionStrategy == ResolutionStrategy.ByArc)
                {
                    EditorGUILayout.PropertyField(cornerResolution, new GUIContent("Corner Resolution"));
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
