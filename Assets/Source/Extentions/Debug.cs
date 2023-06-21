using UnityEngine;

public class Debug : UnityEngine.Debug
{
    public static void DrawSquare(Vector3 position, Quaternion rotation, Vector2 halfSize, Color color)
    {
        Vector3 leftUp = position + rotation * new Vector2(-halfSize.x, halfSize.y);
        Vector3 leftDown = position + rotation * new Vector2(-halfSize.x, -halfSize.y);
        Vector3 rightUp = position + rotation * new Vector2(halfSize.x, halfSize.y);
        Vector3 rightDown = position + rotation * new Vector2(halfSize.x, -halfSize.y);

        DrawLine(leftUp, leftDown, color);
        DrawLine(leftDown, rightDown, color);
        DrawLine(rightDown, rightUp, color);
        DrawLine(rightUp, leftUp, color);
    }

    public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 halfSize, Color color)
    {
        Vector3 offsetX = rotation * Vector3.Scale(Vector3.right, halfSize);
        Vector3 offsetY = rotation * Vector3.Scale(Vector3.up, halfSize);
        Vector3 offsetZ = rotation * Vector3.Scale(Vector3.forward, halfSize);

        Vector3 pointA = -offsetX + offsetY;
        Vector3 pointB = offsetX + offsetY;
        Vector3 pointC = offsetX - offsetY;
        Vector3 pointD = -offsetX - offsetY;

        DrawSquare(position - offsetZ, rotation, halfSize, color);
        DrawSquare(position + offsetZ, rotation, halfSize, color);

        DrawLine(position + pointA - offsetZ, position + pointA + offsetZ, color);
        DrawLine(position + pointB - offsetZ, position + pointB + offsetZ, color);
        DrawLine(position + pointC - offsetZ, position + pointC + offsetZ, color);
        DrawLine(position + pointD - offsetZ, position + pointD + offsetZ, color);
    }

    public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, Color color, int segments)
    {
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        float angleStep = (360.0f / segments);

        angleStep *= Mathf.Deg2Rad;

        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        Vector3 zPos = Vector3.forward * position.z;

        for (int i = 0; i < segments; i++)
        {
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);
            lineStart.z = 0.0f;

            int nextStep = i + 1;

            lineEnd.x = Mathf.Cos(angleStep * nextStep);
            lineEnd.y = Mathf.Sin(angleStep * nextStep);
            lineEnd.z = 0.0f;

            lineStart *= radius;
            lineEnd *= radius;

            lineStart = rotation * lineStart;
            lineEnd = rotation * lineEnd;

            lineStart += position;
            lineEnd += position;

            DrawLine(lineStart, lineEnd, color);
        }
    }

    public static void DrawSphere(Vector3 position, Quaternion orientation, float radius, Color color, int segments = 4)
    {
        if (segments < 2)
        {
            segments = 2;
        }

        int doubleSegments = segments * 2;
        float meridianStep = 180.0f / segments;

        for (int i = 0; i < segments; i++)
        {
            DrawCircle(position, Quaternion.Euler(0, meridianStep * i, 0), radius, color, doubleSegments);
        }
    }
}
