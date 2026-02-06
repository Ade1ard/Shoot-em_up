using UnityEditor;

[CustomEditor(typeof(ObjectMovement))]
public class ObjectMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty MovementTypeProperty = serializedObject.FindProperty("_movementType");
        EditorGUILayout.PropertyField(MovementTypeProperty);
        MovementType currentMovementType = (MovementType)MovementTypeProperty.enumValueIndex;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isItEnemy"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_upDirection"));

        switch (currentMovementType)
        {
            case MovementType.Linear:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_speed"));
                break;

            case MovementType.Curvelinear:
                CurveLinearEditor();
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void CurveLinearEditor()
    {
        SerializedProperty TrajectoryTypeProperty = serializedObject.FindProperty("_type");
        EditorGUILayout.PropertyField(TrajectoryTypeProperty);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_moveDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_moveDurationOffset"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pathType"));

        TrajectoryType currentTrajectoryType = (TrajectoryType)TrajectoryTypeProperty.enumValueIndex;

        switch (currentTrajectoryType)
        {
            case TrajectoryType.SineWave:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amplitude"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("frequency"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sinDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("resolution"));
                break;

            case TrajectoryType.Circle:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("loops"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("clockwise"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("circleResolution"));
                break;

            case TrajectoryType.Spiral:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralStartRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralEndRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralTurns"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spiralDistance"));
                break;
        }
    }
}