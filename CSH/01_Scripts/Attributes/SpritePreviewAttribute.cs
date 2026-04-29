using UnityEngine;

namespace CSH.Scripts.Attributes
{
    public class SpritePreviewAttribute : PropertyAttribute
    {
        public readonly float Height;
        public readonly float Padding;

        public SpritePreviewAttribute(float height = 100f, float padding = 4f)
        {
            Height = height;
            Padding = padding;
        }
    }
}