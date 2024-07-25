using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class UIGridRenderer : Graphic
    {
        public float interiorThickness = 1f;
        // public float exteriorThickness = 2f;
        // public float interiorAlphaScale = .5f;

        public Vector2Int gridSize;
        public Vector2 unitScaling = new Vector2(1, 1);

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float height = rectTransform.rect.height;
            float width = rectTransform.rect.width;
            float cellWidth = width / (float)this.gridSize.x;
            float cellHeight = height / (float)this.gridSize.y;
            // Color exteriorColor = color;
            // exteriorColor.a *= interiorAlphaScale;

            vh.Clear();


            int count = 0;
            for (int y = 0; y < gridSize.y; ++y)
            {
                for (int x = 0; x < gridSize.x; ++x)
                {
                    UIGraphicLibrary.DrawCell(vh, x, y, count, cellWidth, cellHeight, this.interiorThickness, color);
                    ++count;
                }
            }
        }
    }
}
