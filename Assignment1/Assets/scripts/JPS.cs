using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class JPS : MonoBehaviour {
//
// 	// Use this for initialization
// 	void Start () {
// 		
// 	}
// 	
// 	// Update is called once per frame
// 	void Update () {
// 		
// 	}
// }

public class JPS
{
    static private PriorityQueue<int, Pos> m_openList = new PriorityQueue<int, Pos>();
    static private int[,] F;
    static private int[,] G;
    static private int[,] H;
    static private bool[,] closeList;
    static private Pos[,] parent;

    static private Pos[] offset =
    {
        new Pos(-1, 0),
        new Pos(0, -1),
        new Pos(1, 0),
        new Pos(0, 1),
        new Pos(-1, 1),
        new Pos(-1, -1),
        new Pos(1, -1),
        new Pos(1, 1)
    };

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
                if (!(tmp.x < 0 || tmp.x >= m || tmp.y < 0 || tmp.y >= n || closeList[tmp.x, tmp.y] ||
                      matrix[tmp.x, tmp.y] < 0))
                {
                    if (parent[item.Value.x, item.Value.y] == tmp)
                    {
                        continue;
                    }

                    int cost = 0;
                    int flag = 0;
                    //////////////
                    if (i < 4)
                    {
                        Pos tmpL = tmp + offset[(i + 1) % 4];
                        Pos tmpR = tmp + offset[(i + 3) % 4];
                        cost = G[item.Value.x, item.Value.y] + 10;

                        if (G[tmp.x, tmp.y] == -1 || G[tmp.x, tmp.y] > cost)
                        {
                            flag = G[tmp.x, tmp.y] == -1 ? 1 : 2;
                            G[tmp.x, tmp.y] = cost;
                            F[tmp.x, tmp.y] = G[tmp.x, tmp.y] + H[tmp.x, tmp.y];
                            parent[tmp.x, tmp.y] = new Pos(item.Value.x, item.Value.y);
                        }
                        
                        while (true)
                        {
                            if (tmp == end)
                            {
                                IsShorter(tmp, flag);
                                break;
                            }
                            

                            Pos tmpF = tmp + offset[i];
                            Pos tmpLF = tmpL + offset[i];
                            Pos tmpRF = tmpR + offset[i];

                            if (tmpF.x < 0 || tmpF.x >= m || tmpF.y < 0 || tmpF.y >= n || closeList[tmpF.x, tmpF.y] ||
                                matrix[tmpF.x, tmpF.y] < 0)
                            {
                                IsShorter(tmp, flag);
                                break;
                            }
                            
                            

                            if (!(tmpLF.x < 0 || tmpLF.x >= m || tmpLF.y < 0 || tmpLF.y >= n))
                            {
                                if (matrix[tmpL.x, tmpL.y] < 0 && matrix[tmpLF.x, tmpLF.y] >= 0)
                                {
                                    IsShorter(tmp, flag);
                                    break;
                                }
                            }

                            if (!(tmpRF.x < 0 || tmpRF.x >= m || tmpRF.y < 0 || tmpRF.y >= n))
                            {
                                if (matrix[tmpR.x, tmpR.y] < 0 && matrix[tmpRF.x, tmpRF.y] >= 0)
                                {
                                    IsShorter(tmp, flag);
                                    break;
                                }
                            }
                            cost += 10;
                            if (G[tmpF.x, tmpF.y] == -1 || G[tmpF.x, tmpF.y] > cost)
                            {
                                flag = G[tmpF.x, tmpF.y] == -1 ? 1 : 2;
                                G[tmpF.x, tmpF.y] = cost;
                                F[tmpF.x, tmpF.y] = G[tmpF.x, tmpF.y] + H[tmpF.x, tmpF.y];
                                parent[tmpF.x, tmpF.y] = new Pos(tmp.x, tmp.y);
                            }
                            tmp = tmpF;
                            tmpL = tmpLF;
                            tmpR = tmpRF;
                            
                        }
                    }

                    else
                    {
                        cost = G[item.Value.x, item.Value.y] + 14;
                        if (G[tmp.x, tmp.y] == -1 || G[tmp.x, tmp.y] > cost)
                        {
                            flag = G[tmp.x, tmp.y] == -1 ? 1 : 2;
                            G[tmp.x, tmp.y] = cost;
                            F[tmp.x, tmp.y] = G[tmp.x, tmp.y] + H[tmp.x, tmp.y];
                            parent[tmp.x, tmp.y] = new Pos(item.Value.x, item.Value.y);
                        }                     
                        while (true)
                        {
                            if (tmp == end)
                            {
                                IsShorter(tmp, flag);
                                break;
                            }

                            Pos tmpF = tmp + offset[i];
                            Pos tmpL = tmp + offset[i + 1 > 7 ? i - 3 : i + 1];
                            Pos tmpR = tmp + offset[i - 1 < 4 ? i + 3 : i - 1];
                            Pos tmpLF = tmp + offset[i - 4];
                            Pos tmpRF = tmp + offset[i - 5 < 0 ? i - 1 : i - 5];
                            if (tmpF.x < 0 || tmpF.x >= m || tmpF.y < 0 || tmpF.y >= n || closeList[tmpF.x, tmpF.y] ||
                                matrix[tmpF.x, tmpF.y] < 0)
                            {
                                IsShorter(tmp, flag);
                                break;
                            }
                            
                            if (!(tmpL.x < 0 || tmpL.x >= m || tmpL.y < 0 || tmpL.y >= n))
                            {
                                if (matrix[tmpL.x, tmpL.y] >= 0)
                                {
                                    IsShorter(tmp, flag);
                                    break;
                                }
                            }
                            if (!(tmpR.x < 0 || tmpR.x >= m || tmpR.y < 0 || tmpR.y >= n))
                            {
                                if (matrix[tmpR.x, tmpR.y] >= 0)
                                {
                                    IsShorter(tmp, flag);
                                    break;
                                }
                            }


                            if (!(tmpLF.x < 0 || tmpLF.x >= m || tmpLF.y < 0 || tmpLF.y >= n))
                            {
                                if (matrix[tmpLF.x, tmpLF.y] >= 0)
                                {
                                    IsShorter(tmp, flag);
                                    break;
                                }
                            }

                            if (!(tmpRF.x < 0 || tmpRF.x >= m || tmpRF.y < 0 || tmpRF.y >= n))
                            {
                                if (matrix[tmpRF.x, tmpRF.y] >= 0)
                                {
                                    IsShorter(tmp, flag);
                                    break;
                                }
                            }
                            cost += 14;
                            if (G[tmpF.x, tmpF.y] == -1 || G[tmpF.x, tmpF.y] > cost)
                            {
                                flag = G[tmpF.x, tmpF.y] == -1 ? 1 : 2;
                                G[tmpF.x, tmpF.y] = cost;
                                F[tmpF.x, tmpF.y] = G[tmpF.x, tmpF.y] + H[tmpF.x, tmpF.y];
                                parent[tmpF.x, tmpF.y] = new Pos(tmp.x, tmp.y);
                            }
                            tmp = tmpF;
                            
                        }
                    }


                }
            }
        }

// UpdatePath(matrix, startX, startY, parent[endX, endY].x, parent[endX, endY].y);
        return G[endX, endY];
    }

    static private int GuessDis(int posX, int posY, int endX, int endY)
    {
        int disX = Math.Abs(endX - posX);
        int disY = Math.Abs(endY - posY);

        int minDir = Math.Min(disX, disY);
        return 14 * minDir + 10 * ((disX - minDir) + (disY - minDir));
    }

    static private void IsShorter(Pos pos, int flag)
    {
        if (flag == 1)
        {
            m_openList.Insert(F[pos.x, pos.y], pos);
        }
        else if(flag == 2) 
        {
            m_openList.Promote(pos, F[pos.x, pos.y]);
        }
    }
}