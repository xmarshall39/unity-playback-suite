using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class UIGraphRenderer : Graphic
    {
        public UIGridRenderer gridRenderer;
        public List<Vector2> points;
        public float lineThickness;

        public Vector2Int gridSize;
        public Vector2 gridScaling;

        float width;
        float height;
        float unitWidth;
        float unitHeight;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            width = rectTransform.rect.width;
            height = rectTransform.rect.height;

            unitWidth = width / gridSize.x;
            unitHeight = height / gridSize.y;

            if (points.Count < 2) return;


            float angle = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {

                Vector2 point = points[i];
                Vector2 point2 = points[i + 1];

                if (i < points.Count - 1)
                {
                    angle = GetAngle(points[i], points[i + 1]) + 90f;
                }

                DrawVerticesForPoint(point, point2, angle, vh);
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int index = i * 4;
                vh.AddTriangle(index + 0, index + 1, index + 2);
                vh.AddTriangle(index + 1, index + 2, index + 3);

                //Stich the corners where lines meet
                if( i < points.Count - 2)
                {
                    int nextIndex = (i + 1) * 4;
                    vh.AddTriangle(index + 2, index + 3, nextIndex + 0);
                    vh.AddTriangle(index + 2, index + 3, nextIndex + 1);
                }
            }
        }

        public float GetAngle(Vector2 me, Vector2 target)
        {
            //panel resolution go there in place of 9 and 16

            return (float)(Mathf.Atan2(9f * (target.y - me.y), 16f * (target.x - me.x)) * (180 / Mathf.PI));
        }

        void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-lineThickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(lineThickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-lineThickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(lineThickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
            vh.AddVert(vertex);
        }

        //Unused non-trig method
        void DrawLine(Vector2 point0, Vector2 point1, VertexHelper vh, ref int index0)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            Vector3 shiftV = Rotate90CW(point1 - point0).normalized * lineThickness;
            vertex.position = new Vector3(unitWidth * point0.x, unitHeight * point0.y);
            vh.AddVert(vertex);
            vertex.position += shiftV;
            vh.AddVert(vertex);

            vertex.position = new Vector3(unitWidth * point1.x, unitHeight * point1.y);
            vh.AddVert(vertex);
            vertex.position += shiftV;
            vh.AddVert(vertex);

            vh.AddTriangle(index0 + 0, index0 + 2, index0 + 3);
            vh.AddTriangle(index0 + 1, index0 + 0, index0 + 3);
            index0 += 4;
        }

        Vector3 Rotate90CW(Vector3 _dir)
        {
            return new Vector3(_dir.y, -_dir.x);
        }

        protected void Update()
        {
            if (gridRenderer)
            {
                gridSize = gridRenderer.gridSize;
                gridScaling = gridRenderer.unitScaling;
            }
        }
    }
}

