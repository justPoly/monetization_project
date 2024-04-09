using UnityEditor;

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIShadow editor.
    /// </summary>
    [CustomEditor(typeof(UIShadow))]
    [CanEditMultipleObjects]
    public class UIShadowEditor : Editor
    {
        UIEffect uiEffect;
        SerializedProperty _spStyle;
        SerializedProperty _spEffectDistance;
        SerializedProperty _spEffectColor;
        SerializedProperty _spUseGraphicAlpha;
        SerializedProperty _spBlurFactor;

        SerializedProperty m_enablePulsing;
        SerializedProperty m_pulsingSpeed;
        SerializedProperty maxAlpha;
        SerializedProperty minAlpha;

        void OnEnable()
        {
            uiEffect = (target as UIShadow).GetComponent<UIEffect>();
            _spStyle = serializedObject.FindProperty("m_Style");
            _spEffectDistance = serializedObject.FindProperty("m_EffectDistance");
            _spEffectColor = serializedObject.FindProperty("m_EffectColor");
            _spUseGraphicAlpha = serializedObject.FindProperty("m_UseGraphicAlpha");
            _spBlurFactor = serializedObject.FindProperty("m_BlurFactor");

            m_enablePulsing = serializedObject.FindProperty("m_enablePulsing");
            m_pulsingSpeed = serializedObject.FindProperty("m_pulsingSpeed");
            maxAlpha = serializedObject.FindProperty("maxAlpha");
            minAlpha = serializedObject.FindProperty("minAlpha");
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //================
            // Shadow setting.
            //================
            EditorGUILayout.PropertyField(_spStyle);

            // When shadow is enable, show parameters.
            if (_spStyle.intValue != (int)ShadowStyle.None)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_spEffectDistance);
                EditorGUILayout.PropertyField(_spEffectColor);
                EditorGUILayout.PropertyField(_spUseGraphicAlpha);

                if (uiEffect && uiEffect.blurMode != BlurMode.None)
                {
                    EditorGUILayout.PropertyField(_spBlurFactor);
                }

                EditorGUILayout.PropertyField(m_enablePulsing);
                if (((UIShadow)target).EnablePulsing)
                {
                    EditorGUILayout.PropertyField(m_pulsingSpeed);
                    EditorGUILayout.PropertyField(maxAlpha);
                    EditorGUILayout.PropertyField(minAlpha);
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
