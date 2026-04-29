

namespace CSH.Scripts.Attributes
{
    using UnityEditor;
    using UnityEditor.Sprites;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;

            if (!IsExactSpriteField(property))
                return lineHeight;

            Sprite sprite = property.objectReferenceValue as Sprite;
            if (sprite == null)
                return lineHeight;

            SpritePreviewAttribute previewAttribute = (SpritePreviewAttribute)attribute;
            return lineHeight + previewAttribute.Padding + previewAttribute.Height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect fieldRect = new Rect(position.x, position.y, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, property, label);

            if (IsExactSpriteField(property))
            {
                Sprite sprite = property.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    SpritePreviewAttribute previewAttribute = (SpritePreviewAttribute)attribute;

                    Rect previewRect = new Rect(
                        position.x,
                        fieldRect.yMax + previewAttribute.Padding,
                        position.width,
                        previewAttribute.Height
                    );

                    DrawPreview(previewRect, sprite);
                }
            }

            EditorGUI.EndProperty();
        }

        private bool IsExactSpriteField(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference
                   && fieldInfo != null
                   && fieldInfo.FieldType == typeof(Sprite);
        }

        private void DrawPreview(Rect rect, Sprite sprite)
        {
            EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f, 1f));

            Texture2D texture = SpriteUtility.GetSpriteTexture(sprite, false);
            if (texture == null)
            {
                EditorGUI.LabelField(rect, "Sprite texture not found");
                return;
            }

            Rect spriteRect = sprite.rect;

            Rect uv = new Rect(
                spriteRect.x / texture.width,
                spriteRect.y / texture.height,
                spriteRect.width / texture.width,
                spriteRect.height / texture.height
            );

            Rect drawRect = FitRect(rect, spriteRect.width / spriteRect.height);

            GUI.DrawTextureWithTexCoords(drawRect, texture, uv, true);
        }

        private Rect FitRect(Rect rect, float imageAspect)
        {
            float rectAspect = rect.width / rect.height;

            if (imageAspect > rectAspect)
            {
                float height = rect.width / imageAspect;
                float y = rect.y + (rect.height - height) * 0.5f;
                return new Rect(rect.x, y, rect.width, height);
            }
            else
            {
                float width = rect.height * imageAspect;
                float x = rect.x + (rect.width - width) * 0.5f;
                return new Rect(x, rect.y, width, rect.height);
            }
        }
    }
}

