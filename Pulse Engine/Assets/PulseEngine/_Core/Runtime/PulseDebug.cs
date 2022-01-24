using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PulseDebug
{
    #region Loggers >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Ecrit un message en console.
    /// </summary>
    /// <param name="_message"></param>
    public static void Log<T>(T _message)
    {
        Debug.Log(_message);
    }

    /// <summary>
    /// Ecrit un message d'alerte en console.
    /// </summary>
    /// <param name="_message"></param>
    public static void LogWarning<T>(T _message)
    {
        Debug.LogWarning(_message);
    }

    /// <summary>
    /// Ecrit un message d'erreur en console.
    /// </summary>
    /// <param name="_message"></param>
    public static void LogError<T>(T _message)
    {
        Debug.LogError(_message);
    }

    #endregion

    #region Graphic >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Dessine une ligne entre A et B.
    /// </summary>
    public static void DrawRLine(Vector3 A, Vector3 B, Color color = default)
    {
        Debug.DrawLine(A, B, color);
    }

    /// <summary>
    /// Dessine une rayon de A dans la direction DIR
    /// </summary>
    public static void DrawRay(Vector3 A, Vector3 Dir, Color color = default)
    {
        Debug.DrawRay(A, Dir, color);
    }

    /// <summary>
    /// Dessine un Cube de centre A , de taille S et de Rotation R
    /// </summary>
    public static void DrawCube(Vector3 A, Vector3 S, Quaternion R, Color color = default)
    {
        Vector3[] cube = new Vector3[8]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),//0
            new Vector3(0.5f, -0.5f, -0.5f),//1
            new Vector3(0.5f, 0.5f, -0.5f),//2
            new Vector3(-0.5f, 0.5f, -0.5f),//3
            new Vector3(-0.5f, 0.5f, 0.5f),//4
            new Vector3(0.5f, 0.5f, 0.5f),//5
            new Vector3(0.5f, -0.5f, 0.5f),//6
            new Vector3(-0.5f, -0.5f, 0.5f),//7
        };
        Vector3[] points = new Vector3[cube.Length];

        for (int i = 0, len = points.Length; i < len; i++)
        {
            var pt = cube[i];
            var trsMatrix = Matrix4x4.TRS(A, R, S);
            points[i] = trsMatrix.MultiplyPoint3x4(pt);
            if (i > 0 && i < len)
                Debug.DrawLine(points[i], points[i - 1], color);
            if (i >= len - 1)
            {
                Debug.DrawLine(points[i], points[0], color);
                Debug.DrawLine(points[i], points[4], color);
            }
            if(i == 3)
                Debug.DrawLine(points[i], points[0], color);
            if(i == 5)
                Debug.DrawLine(points[i], points[2], color);
            if(i == 6)
                Debug.DrawLine(points[i], points[1], color);
        }
    }

    /// <summary>
    /// Dessine un cercle de centre A et de rayon R, aligned with normal N
    /// </summary>
    public static void DrawCircle(Vector3 A, float R, Vector3 N, Color color = default, int tickness = 30)
    {
        int step = 360 / tickness;
        Vector3[] points = new Vector3[step];
        for (int i = 0, len = points.Length; i < len; i++)
        {
            float xComp = R * Mathf.Cos((i * tickness) * Mathf.Deg2Rad);
            float yComp = R * Mathf.Sin((i * tickness) * Mathf.Deg2Rad);
            var pt = new Vector3(xComp, yComp, 0);
            var trsMatrix = Matrix4x4.TRS(A, Quaternion.LookRotation(N), Vector3.one);
            points[i] = trsMatrix.MultiplyPoint3x4(pt);
            if (i > 0 && i < len)
                Debug.DrawLine(points[i], points[i - 1], color);
            if (i >= len - 1)
                Debug.DrawLine(points[i], points[0], color);
        }
    }

    /// <summary>
    /// Display a colored path
    /// </summary>
    /// <param name="_text"></param>
    public static void DrawPath(Vector3[] _path, Color _startColor = default, Color _endColor = default)
    {
        if (_path == null)
            return;
        for (int i = 0; i < _path.Length - 1; i++)
        {
            Color c = Color.Lerp(_startColor, _endColor, Mathf.InverseLerp(0, _path.Length - 1, i));
            DrawRLine(_path[i], _path[i + 1], c);
        }
    }

    /// <summary>
    /// Affiche du texte en debug
    /// </summary>
    /// <param name="_text"></param>
    public static void DrawText(string _text, float _font, Color _color = default)
    {

    }

    /// <summary>
    /// Transforme un charactere en nuage de points.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static Vector3[] PointsFromChar(char c)
    {
        switch (c.ToString().ToUpper().ToCharArray()[0])
        {
            default:
                return null;
        }
    }

    #endregion
}
