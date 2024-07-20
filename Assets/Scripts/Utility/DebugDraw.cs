using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw 
{
    public const float TAU = 6.283185307179586f;
    public static Vector2 GetUnitVectorByAngle(float angleRad)
    {
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    void DrawCircle(Vector2 center, float radius, int segments)
    {
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            Vector2 start = center + new Vector2(x, y);

            angle += 360f / segments;

            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            Vector2 end = center + new Vector2(x, y);

            Debug.DrawLine(start, end, Color.red, 1f);
        }
    }

    public static void DrawWireCircle(Vector3 pos, Quaternion rot, float radius, Color color, int detail = 32)
    {
        Vector3[] points3D = new Vector3[detail];
        for (int i = 0; i < detail; ++i)
        {
            float t = (float)(i) / detail;
            float angRad = TAU * t;
            Vector2 points2D = GetUnitVectorByAngle(angRad);
            points2D *= radius;
            points3D[i] = pos + rot * points2D;
        }

        for (int i = 0; i < detail - 1; ++i)
        {
            Debug.DrawLine(points3D[i], points3D[i + 1], color);
        }
        Debug.DrawLine(points3D[detail - 1], points3D[0], color);
    }
}
