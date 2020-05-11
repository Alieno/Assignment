using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pos
{
    public int x;
    public int y;

    public Pos(int a, int b)
    {
        x = a;
        y = b;
    }

    public static Pos operator +(Pos a, Pos b)
    {
        return new Pos(a.x + b.x, a.y + b.y);
    }

    public static bool operator ==(Pos a, Pos b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Pos a, Pos b)
    {
        return !(a == b);
    }
}
public class Less<T> : IComparer
{
    int IComparer.Compare(object x, object y)
    {
        return -Comparer<T>.Default.Compare((T)x, (T)y);
    }
}
public class AStar
{
    static private PriorityQueue<int, Pos> m_openList = new PriorityQueue<int, Pos>();
    static private int[,] F;
    static private int[,] G;
    static private int[,] H;
    static private bool[,] closeList;
    static private Pos[,] parent;

    static private Pos[] offset = { new Pos(-1, 0),
                                    new Pos(0, -1),
                                    new Pos(0, 1),
                                    new Pos(1, 0),
                                    new Pos(-1, -1),
                                    new Pos(-1, 1),
                                    new Pos(1, -1),
                                    new Pos(1, 1)};

    static public List<Pos> OpenList; 
    
    static public Pos[,] Parent
    {
        get { return parent; }
    }

    public static int Calculate(int[,] matrix, int startX, int startY, int endX, int endY)
    {
        int m = matrix.GetLength(0);
        if (m == 0)
        {
            return 0;
        }
        int n = matrix.GetLength(1);
        if (n == 0)
        {
            return 0;
        }
        Debug.Assert(startX >= 0 && startX < m);
        Debug.Assert(startY >= 0 && startY < n);
        Debug.Assert(endX >= 0 && endX < m);
        Debug.Assert(endY >= 0 && endY < n);

        F = new int[m, n];
        G = new int[m, n];
        H = new int[m, n];
        closeList = new bool[m, n];
        parent = new Pos[m, n];
        OpenList = new List<Pos>();
        m_openList = new PriorityQueue<int, Pos>(new Less<int>());
        for (int i = 0; i < m; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                F[i, j] = -1;
                G[i, j] = -1;
                H[i, j] = GuessDis(i, j, endX, endY);
                closeList[i, j] = false;
                parent[i, j] = new Pos(i, j);
            }
        }

        parent[startX, startY] = new Pos(startX, startY);
        G[startX, startY] = 0;
        F[startX, startY] = G[startX, startY] + H[startX, startY];
        Pos start = new Pos(startX, startY);
        Pos end = new Pos(endX, endY);
        m_openList.Insert(F[startX, startY], start);
        while (!m_openList.Empty())
        {
            var item = m_openList.Extract();
            if (item.Value == end)
            {
                break;
            }
            closeList[item.Value.x, item.Value.y] = true;

            for (int i = 0; i < offset.Length; ++i)
            {
                Pos tmp = item.Value + offset[i];
                if (tmp.x < 0 || tmp.x >= m || tmp.y < 0 || tmp.y >= n)
                {
                    continue;
                }
                if (closeList[tmp.x, tmp.y] || matrix[tmp.x, tmp.y] < 0)
                {
                    continue;
                }
                int cost = G[item.Value.x, item.Value.y] + (i >= 4 ? 14 : 10);
                if (G[tmp.x, tmp.y] == -1)
                {
                    G[tmp.x, tmp.y] = cost;
                    F[tmp.x, tmp.y] = G[tmp.x, tmp.y] + H[tmp.x, tmp.y];
                    m_openList.Insert(F[tmp.x, tmp.y], tmp);
                    OpenList.Add(tmp);
                    parent[tmp.x, tmp.y] = new Pos(item.Value.x, item.Value.y);
                }
                else if (G[tmp.x, tmp.y] > cost)
                {
                    G[tmp.x, tmp.y] = cost;
                    F[tmp.x, tmp.y] = G[tmp.x, tmp.y] + H[tmp.x, tmp.y];
                    m_openList.Promote(tmp, F[tmp.x, tmp.y]);
                    parent[tmp.x, tmp.y] = new Pos(item.Value.x, item.Value.y);
                }
            }
        }

        // UpdatePath(matrix, startX, startY, parent[endX, endY].x, parent[endX, endY].y);

        return G[endX, endY];
    }

    static void UpdatePath(int[,] matrix, int startX, int startY, int x, int y)
    {
        if (x == startX && y == startY)
        {
            return;
        }

        matrix[x, y] = 3;
        UpdatePath(matrix, startX, startY, parent[x, y].x, parent[x,y].y);
    }

    static private int GuessDis(int posX, int posY, int endX, int endY)
    {
        int disX = Math.Abs(endX - posX);
        int disY = Math.Abs(endY - posY);
        int minDir = Math.Min(disX, disY);
        return 14 * minDir + 10 * ((disX - minDir) + (disY - minDir));
    }
}
