using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    public Vector2[] Points;

    public float LineThickness = 2f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (Points == null || Points.Length < 2)
        {
            return;
        }

        float width = LineThickness / 2;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Vector2 point1 = Points[i];
            Vector2 point2 = Points[i + 1];
            Vector2 direction = (point2 - point1).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * width;

            vh.AddVert(point1 - normal, color, new Vector2(0, 0));
            vh.AddVert(point1 + normal, color, new Vector2(0, 1));
            vh.AddVert(point2 + normal, color, new Vector2(1, 1));
            vh.AddVert(point2 - normal, color, new Vector2(1, 0));

            int offset = i * 4;
            vh.AddTriangle(offset, offset + 1, offset + 2);
            vh.AddTriangle(offset + 2, offset + 3, offset);
        }
    }
}
