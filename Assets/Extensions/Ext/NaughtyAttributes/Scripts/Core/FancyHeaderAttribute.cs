using UnityEditor;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]

public class FancyHeaderAttribute : PropertyAttribute

{

    public readonly string header;

    public readonly string colorString;

    public readonly Color color;

    public readonly float textHeightIncrease;

    public readonly float textYPosition;
    public FancyHeaderAttribute(string header, string colorString) : this(header, 1, colorString) { }


    public FancyHeaderAttribute(string header, float textHeightIncrease = 1.5f, string colorString =
    "lightblue", float textPosition = 5.5f)
    {
        this.header = header;
        this.colorString = colorString;
        this.textYPosition = textPosition;

        this.textHeightIncrease = Mathf.Max(1f, textHeightIncrease);

        if (string.IsNullOrEmpty(header))
            this.textHeightIncrease = 1f;

        if (ColorUtility.TryParseHtmlString(colorString, out this.color)) return;

        this.color = new Color(173, 216, 230);
        this.colorString = "lightblue";

    }
}
#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(FancyHeaderAttribute))]
public class HeaderDrawer : DecoratorDrawer

{

    #region Overrides of DecoratorDrawer

    /// <tnheritdoc />
    public override void OnGUI(Rect position)

    {

        if (!(attribute is FancyHeaderAttribute headerAttribute)) return;

        position = EditorGUI.IndentedRect(position);
        position.yMin += EditorGUIUtility.singleLineHeight * ((headerAttribute.textHeightIncrease / 2));

        if (string.IsNullOrEmpty(headerAttribute.header))
        {
            position.height = headerAttribute.textHeightIncrease;
            EditorGUI.DrawRect(position, headerAttribute.color);
            return;

        }

        GUIStyle style = new GUIStyle(EditorStyles.label) { richText = true };
        GUIContent label = new GUIContent(
        $"<color={headerAttribute.colorString}><size={style.fontSize + headerAttribute.textHeightIncrease}><b>{headerAttribute.header}</b></size></color>");

        Vector2 textSize = style.CalcSize(label);
        float separatorWidth = (position.width - textSize.x) / 2.0f;

        Rect prefixRect = new Rect(position.xMin - 5f, position.yMin + headerAttribute.textYPosition, separatorWidth, headerAttribute.textHeightIncrease);
        Rect labelRect = new Rect(position.xMin + separatorWidth, position.yMin - headerAttribute.textYPosition,
        textSize.x, position.height);
        Rect postRect = new Rect(position.xMin + separatorWidth + 5f + textSize.x, position.yMin + headerAttribute.textYPosition,
        separatorWidth, headerAttribute.textHeightIncrease);
        EditorGUI.DrawRect(prefixRect, headerAttribute.color);
        EditorGUI.LabelField(labelRect, label, style);
        EditorGUI.DrawRect(postRect, headerAttribute.color);

    }
    public override float GetHeight()

    {
        FancyHeaderAttribute headerAttribute = attribute as FancyHeaderAttribute;
        return EditorGUIUtility.singleLineHeight * (headerAttribute?.textHeightIncrease + 0.5f ?? 0);

    }

    #endregion
}
#endif