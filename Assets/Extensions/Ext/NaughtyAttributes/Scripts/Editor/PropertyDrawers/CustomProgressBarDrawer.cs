using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomProgressBarAttribute))]
public class CustomProgressBarDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Float)
        {
            GUI.Label(position, "ERROR: can only apply progress bar onto a float");
            return;
        }

        if ((attribute as CustomProgressBarAttribute).hideWhenZero && property.floatValue <= 0)
            return;

        var dynamicLabel = property.serializedObject.FindProperty((attribute as CustomProgressBarAttribute).label);

        //position.Set(position.x, position.y, position.width, (position.height * 2));
        EditorGUI.ProgressBar(position, property.floatValue / 1f, dynamicLabel == null ? property.name : dynamicLabel.stringValue);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if ((attribute as CustomProgressBarAttribute).hideWhenZero && property.floatValue <= 0)
            return 0;

        return base.GetPropertyHeight(property, label);
    }
}
