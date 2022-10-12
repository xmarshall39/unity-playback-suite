using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public static class UIGraphicLibrary
    {
        public static void DrawCell(VertexHelper vh, int x, int y, int index, float cellWidth, float cellHeight, float marginSize, Color fillColor)
        {
            float xPos = cellWidth * x;
            float yPos = cellHeight * y;

            UIVertex currentVert = UIVertex.simpleVert;
            currentVert.position = new Vector3(xPos, yPos);
            currentVert.color = fillColor;

            vh.AddVert(currentVert);

            currentVert.position = new Vector3(xPos, yPos + cellHeight);
            vh.AddVert(currentVert);

            currentVert.position = new Vector3(xPos + cellWidth, yPos + cellHeight);
            vh.AddVert(currentVert);

            currentVert.position = new Vector3(xPos + cellWidth, yPos);
            vh.AddVert(currentVert);

            float sqrWidth = marginSize * marginSize;
            float sqrDistance = sqrWidth / 2f;
            float distance = Mathf.Sqrt(sqrDistance);
            int offset = index * 8;

            currentVert.position = new Vector3(xPos + distance, yPos + distance);
            vh.AddVert(currentVert);

            currentVert.position = new Vector3(xPos + distance, yPos + (cellHeight - distance));
            vh.AddVert(currentVert);

            currentVert.position = new Vector3(xPos + (cellWidth - distance), yPos + (cellHeight - distance));
            vh.AddVert(currentVert);

            currentVert.position = new Vector3(xPos + (cellWidth - distance), yPos + distance);
            vh.AddVert(currentVert);

            vh.AddTriangle(offset + 0, offset + 1, offset + 5);
            vh.AddTriangle(offset + 5, offset + 4, offset + 0);

            vh.AddTriangle(offset + 1, offset + 2, offset + 6);
            vh.AddTriangle(offset + 6, offset + 5, offset + 1);

            vh.AddTriangle(offset + 2, offset + 3, offset + 7);
            vh.AddTriangle(offset + 7, offset + 6, offset + 2);

            vh.AddTriangle(offset + 3, offset + 0, offset + 4);
            vh.AddTriangle(offset + 4, offset + 7, offset + 3);
        }
        public static void DrawPoint(VertexHelper vh, List<Vector2> points, Vector4 bounds)
        {
            foreach(Vector2 point in points)
            {

            }
        }
    }
}
