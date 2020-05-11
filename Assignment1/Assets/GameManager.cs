using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	enum MAPMARKER
	{
		NONE = 0,
		START = 1,
		END = 2,
		OBSTACLE = -1,
		PATH = 3,
		OPEN = 4,
	}
	public GameObject Parent;
	public GameObject Box;

	public Toggle ToggleAStar;
	public Toggle ToggleJPS;
	public Toggle ToggleStart;
	public Toggle ToggleEnd;
	public Toggle ToggleObstacle;
	public Toggle ToggleClear;

	public Text TimeUsed;
	public Slider SizeSlider;
	
	private GameObject[,] m_Box = new GameObject[100, 100];
	private int[,] m_Map = new int[100,100];
	
	private bool m_Start = true;
	private bool m_End = false;
	private bool m_Obstacle = false;
	private bool m_Clear = false;
	private bool m_AStar = true;
	private bool m_JPS = false;

	private int m_StartX = -1;
	private int m_StartY = -1;
	private int m_EndX = -1;
	private int m_EndY = -1;

	private int m_Size = 10;

	private int m_IsPlaying = 0;
	private int m_DrawX = 0;
	private int m_DrawY = 0;
	
	void Start () {
		m_IsPlaying = 0;
		for (int i = 0; i < 100; i++)
		{
			for (int j = 0; j < 100; j++)
			{
				GameObject box = Instantiate<GameObject>(Box, Parent.transform);
				box.transform.Translate(new Vector3(i,j,0));
				m_Box[i,j] = box;
				if (i >= m_Size || j >= m_Size)
				{
					m_Map[i, j] = (int) MAPMARKER.OBSTACLE;
					m_Box[i, j].SetActive(false);
				}
				else
				{
					m_Map[i, j] = (int) MAPMARKER.NONE;
				}
			}
		}
		
		Box.SetActive(false);
		DrawColor();
		ToggleAStar.onValueChanged.AddListener((bool isOn)=> { OnToggleAStarClick(isOn); });
		ToggleJPS.onValueChanged.AddListener((bool isOn)=> { OnToggleJPSClick(isOn); });
		ToggleStart.onValueChanged.AddListener((bool isOn)=> { OnToggleStartClick(isOn); });
		ToggleEnd.onValueChanged.AddListener((bool isOn)=> { OnToggleEndClick(isOn); });
		ToggleObstacle.onValueChanged.AddListener((bool isOn)=> { OnToggleObstacleClick(isOn); });
		ToggleClear.onValueChanged.AddListener((bool isOn)=> { OnToggleClearClick(isOn); });

	}



	void Update()
	{
		if (m_IsPlaying > 0)
		{
			UpdatePath();
		}
		OnMapClick();
		DrawColor();
	}

	private void UpdatePath()
	{
		if (m_DrawX == m_StartX && m_DrawY == m_StartY)
		{
			m_IsPlaying = 0;
		}
		else
		{
			if (m_IsPlaying == 1)
			{
				if (m_AStar)
				{
					if (AStar.OpenList.Count > 0)
					{
						if (!(AStar.OpenList.First().x == m_EndX && AStar.OpenList.First().y == m_EndY))
						{
							m_Map[AStar.OpenList.First().x, AStar.OpenList.First().y] = 4;

						}
						AStar.OpenList.RemoveAt(0);
					}
					else
					{
						m_IsPlaying = 2;
					}
				}
			}
			else
			{
				if (!(m_DrawX == m_EndX && m_DrawY == m_EndY))
				{
					m_Map[m_DrawX, m_DrawY] = 3;

				}

				var tmpX = m_DrawX;
				var tmpY = m_DrawY;
				if (m_AStar)
				{
					m_DrawX = AStar.Parent[tmpX, tmpY].x;
					m_DrawY = AStar.Parent[tmpX, tmpY].y;
				}
				else if(m_JPS)
				{
					m_DrawX = JPS.Parent[tmpX, tmpY].x;
					m_DrawY = JPS.Parent[tmpX, tmpY].y;
				}
			
				// Debug.Log(m_DrawX+":"+m_DrawY);
			}
			
		}
	}

	public void OnSliderValueChanged()
	{
		if (m_Size > (int) SizeSlider.value)
		{
			for (int i = (int) SizeSlider.value; i < m_Size; i++)
			{
				for (int j = 0; j < m_Size; j++)
				{
					m_Map[i, j] = (int) MAPMARKER.OBSTACLE;
					m_Map[j, i] = (int) MAPMARKER.OBSTACLE;
					m_Box[i, j].SetActive(false);
					m_Box[j, i].SetActive(false);

				}
			}
		}
		else
		{
			for (int i = m_Size; i < (int) SizeSlider.value; i++)
			{
				for (int j = 0; j < (int) SizeSlider.value; j++)
				{
					m_Map[i, j] = (int) MAPMARKER.NONE;
					m_Map[j, i] = (int) MAPMARKER.NONE;
					m_Box[i, j].SetActive(true);
					m_Box[j, i].SetActive(true);
				}
			}
		}

		m_Size = (int) SizeSlider.value;
		Camera.main.orthographicSize = m_Size / 2f;
		Camera.main.transform.position = new Vector3((m_Size - 1) / 3f, 0.5f * (m_Size - 1), -10);
	}

	private void DrawColor()
	{
		for (int i = 0; i < SizeSlider.value; i++)
		{
			for (int j = 0; j < SizeSlider.value; j++)
			{
				switch (m_Map[i,j])
				{
					case (int) MAPMARKER.START:
						m_Box[i,j].GetComponent<MeshRenderer>().material.color = Color.red;
						break;
					case (int) MAPMARKER.END:
						m_Box[i,j].GetComponent<MeshRenderer>().material.color = Color.blue;
						break;
					case (int) MAPMARKER.OBSTACLE:
						m_Box[i,j].GetComponent<MeshRenderer>().material.color = Color.black;
						break;
					case (int) MAPMARKER.NONE:
						m_Box[i,j].GetComponent<MeshRenderer>().material.color = Color.white;
						break;
					case (int) MAPMARKER.PATH:
						m_Box[i, j].GetComponent<MeshRenderer>().material.color = Color.yellow;
						break;
					case (int) MAPMARKER.OPEN:
						m_Box[i, j].GetComponent<MeshRenderer>().material.color = Color.green;
						break;
				}
			}
		}
	}

	private void OnMapClick()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
		{
			if (Camera.main != null)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hitInfo = Physics2D.Raycast(ray.origin, ray.direction);
				if (hitInfo)
				{
					GameObject hitObj = hitInfo.collider.gameObject;
					int tmpX = (int) hitObj.transform.position.x;
					int tmpY = (int) hitObj.transform.position.y;
					if (m_Start)
					{
						if (m_Map[tmpX,tmpY] == (int) MAPMARKER.NONE)
						{
							if (m_StartX >= 0)
							{
								m_Map[m_StartX,m_StartY] = (int) MAPMARKER.NONE;
							}

							m_StartX = tmpX;
							m_StartY = tmpY;
							m_Map[m_StartX,m_StartY] = (int) MAPMARKER.START;
						}
					}

					if (m_End)
					{
						if (m_Map[tmpX,tmpY] == (int) MAPMARKER.NONE)
						{
							if (m_EndX >= 0)
							{
								m_Map[m_EndX,m_EndY] = (int) MAPMARKER.NONE;
							}

							m_EndX = tmpX;
							m_EndY = tmpY;
							m_Map[m_EndX,m_EndY] = (int) MAPMARKER.END;
						}
					}

					if (m_Obstacle)
					{
						if (m_Map[tmpX,tmpY] != (int) MAPMARKER.START && m_Map[tmpX, tmpY] != (int) MAPMARKER.END)
						{
							m_Map[tmpX,tmpY] = (int) MAPMARKER.OBSTACLE;
						}
					}

					if (m_Clear)
					{
						if (m_Map[tmpX,tmpY] != (int) MAPMARKER.NONE)
						{
							switch (m_Map[tmpX,tmpY])
							{
								case (int) MAPMARKER.START:
									m_StartX = -1;
									m_StartY = -1;
									m_Map[tmpX,tmpY] = (int) MAPMARKER.NONE;
									break;
								case (int) MAPMARKER.END:
									m_EndX = -1;
									m_EndY = -1;
									m_Map[tmpX,tmpY] = (int) MAPMARKER.NONE;
									break;
								case (int) MAPMARKER.OBSTACLE:
									m_Map[tmpX,tmpY] = (int) MAPMARKER.NONE;
									break;
							}
						}
					}
				}
			}
		}

	}

	void OnToggleAStarClick(bool isOn)
	{
		m_AStar = isOn;
	}
	private void OnToggleClearClick(bool isOn)
	{
		m_Clear = isOn;
	}

	private void OnToggleObstacleClick(bool isOn)
	{
		m_Obstacle = isOn;
		
	}

	private void OnToggleEndClick(bool isOn)
	{
		m_End = isOn;
	}

	private void OnToggleStartClick(bool isOn)
	{
		m_Start = isOn;
	}

	private void OnToggleJPSClick(bool isOn)
	{
		m_JPS = isOn;
	}

	public void OnFindPathBtnClick()
	{
		StartCoroutine(CalculatePath());
	}

	IEnumerator CalculatePath()
	{
		for (int i = 0; i < SizeSlider.value; ++i)
		{
			for (int j = 0; j < SizeSlider.value; ++j)
			{
				if (m_Map[i, j] == (int)MAPMARKER.PATH || m_Map[i, j] == (int)MAPMARKER.OPEN)
				{
					m_Map[i, j] = (int)MAPMARKER.NONE;
				}
			}
		}
		DateTime before = DateTime.Now;
		int tmp = 0;
		if (m_AStar)
		{
			tmp = AStar.Calculate(m_Map, m_StartX, m_StartY, m_EndX, m_EndY);
		}
		else if(m_JPS)
		{
			tmp = JPS.Calculate(m_Map, m_StartX, m_StartY, m_EndX, m_EndY);
		}
		DateTime after = DateTime.Now;
		TimeUsed.text = (after - before).ToString();
		Debug.Log(tmp);
		m_IsPlaying = 1;
		m_DrawX = m_EndX;
		m_DrawY = m_EndY;
		yield return null;
	}
}
