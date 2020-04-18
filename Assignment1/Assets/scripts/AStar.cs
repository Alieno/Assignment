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

public class AStar
{
    static private PriorityQueue<int, Pos> m_openList = new PriorityQueue<int, Pos>();
    static private List<List<int>> F = new List<List<int>>();
    static private List<List<int>> G = new List<List<int>>();
    static private List<List<int>> H = new List<List<int>>();
    static private List<List<bool>> closeList = new List<List<bool>>();

    static private Pos[] offset = { new Pos(-1, 0),
                                    new Pos(0, -1),
                                    new Pos(0, 1),
                                    new Pos(1, 0),
                                    new Pos(-1, -1),
                                    new Pos(-1, 1),
                                    new Pos(1, -1),
                                    new Pos(1, 1)};

    public static int Calculate(List<List<int>> matrix, int startX, int startY, int endX, int endY)
    {
        int m = matrix.Count;
        if (m == 0)
        {
            return 0;
        }
        int n = matrix[0].Count;
        if (n == 0)
        {
            return 0;
        }
        Debug.Assert(startX >= 0 && startX < m);
        Debug.Assert(startY >= 0 && startY < n);
        Debug.Assert(endX >= 0 && endX < m);
        Debug.Assert(endY >= 0 && endY < n);

        for (int i = 0; i < m; ++i)
        {
            F.Add(new List<int>());
            G.Add(new List<int>());
            H.Add(new List<int>());
            closeList.Add(new List<bool>());
            for (int j = 0; j < n; ++j)
            {
                F[i].Add(-1);
                G[i].Add(-1);
                H[i].Add(GuessDis(i, j, endX, endY));
                closeList[i].Add(false);
            }
        }

        G[startX][startY] = 0;
        F[startX][startY] = G[startX][startY] + H[startX][startY];
        Pos start = new Pos(startX, startY);
        Pos end = new Pos(endX, endY);
        m_openList.Insert(F[startX][startY], start);
        while (!m_openList.Empty())
        {
            var item = m_openList.Extract();
            if (item.Value == end)
            {
                break;
            }
            closeList[item.Value.x][item.Value.y] = true;

            for (int i = 0; i < offset.Length; ++i)
            {
                Pos tmp = item.Value + offset[i];
                if (tmp.x < 0 || tmp.x >= m || tmp.y < 0 || tmp.y >= n)
                {
                    continue;
                }
                if (closeList[tmp.x][tmp.y] || matrix[tmp.x][tmp.y] < 0)
                {
                    continue;
                }
                int cost = G[item.Value.x][item.Value.y] + (i >= 4 ? 14 : 10);
                if (G[tmp.x][tmp.y] == -1)
                {
                    G[tmp.x][tmp.y] = cost;
                    F[tmp.x][tmp.y] = G[tmp.x][tmp.y] + H[tmp.x][tmp.y];
                    m_openList.Insert(F[tmp.x][tmp.y], tmp);
                }
                else if (G[tmp.x][tmp.y] > cost)
                {
                    G[tmp.x][tmp.y] = cost;
                    F[tmp.x][tmp.y] = G[tmp.x][tmp.y] + H[tmp.x][tmp.y];
                    m_openList.Promote(tmp, F[tmp.x][tmp.y]);
                }
            }
        }

        return G[endX][endY];
    }

    static private int GuessDis(int posX, int posY, int endX, int endY)
    {
        int disX = Math.Abs(endX - posX);
        int disY = Math.Abs(endY - posY);
        int minDir = Math.Min(disX, disY);
        return 14 * minDir + (disX - minDir) + (disY - minDir);
    }
}
