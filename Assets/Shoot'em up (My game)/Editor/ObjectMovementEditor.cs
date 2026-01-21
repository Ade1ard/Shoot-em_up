using UnityEditor;

[CustomEditor(typeof(ObjectMovement))]
public class ObjectMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty typeProperty = serializedObject.FindProperty("type");
        EditorGUILayout.PropertyField(typeProperty);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_moveDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pathType"));

        TrajectoryType currentType = (TrajectoryType)typeProperty.enumValueIndex;

        switch (currentType)
        {
            case TrajectoryType.SineWave:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amplitude"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("frequency"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("resolution"));
                break;

            case TrajectoryType.Circle:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("loops"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("clockwise"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("circleResolution"));
                break;

            case TrajectoryType.ZigZag:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("zigzagWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("zigzagCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("zigzagVerticalDistance"));
                break;

            case TrajectoryType.Spiral:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralStartRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralEndRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralTurns"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}