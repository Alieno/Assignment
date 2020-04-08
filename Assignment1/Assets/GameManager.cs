using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	enum MAPMARKER
	{
		NONE = 0,
		START = 1,
		END = 2,
		OBSTACLE = 3
		
	}
	public GameObject Parent;
	public GameObject Box;

	public Toggle ToggleAStar;
	public Toggle ToggleJPS;
	public Toggle ToggleStart;
	public Toggle ToggleEnd;
	public Toggle ToggleObstacle;
	public Toggle ToggleClear;
	
	
	
	private GameObject[,] m_Box = new GameObject[10, 10];
	private int[,] m_Map = new int[10,10];
	
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
	
	
	void Start () {
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				GameObject box = Instantiate<GameObject>(Box, Parent.transform);
				box.transform.Translate(new Vector3(i,j,0));
				m_Box[i,j] = box;
			}
		}
		Box.SetActive(false);
		ToggleAStar.onValueChanged.AddListener((bool isOn)=> { OnToggleAStarClick(isOn); });
		ToggleJPS.onValueChanged.AddListener((bool isOn)=> { OnToggleJPSClick(isOn); });
		ToggleStart.onValueChanged.AddListener((bool isOn)=> { OnToggleStartClick(isOn); });
		ToggleEnd.onValueChanged.AddListener((bool isOn)=> { OnToggleEndClick(isOn); });
		ToggleObstacle.onValueChanged.AddListener((bool isOn)=> { OnToggleObstacleClick(isOn); });
		ToggleClear.onValueChanged.AddListener((bool isOn)=> { OnToggleClearClick(isOn); });

	}



	void Update()
	{
		OnMapClick();
		DrawColor();
	}

	private void DrawColor()
	{
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
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
						if (m_Map[tmpX, tmpY] == (int) MAPMARKER.NONE)
						{
							if (m_StartX >= 0)
							{
								m_Map[m_StartX, m_StartY] = (int) MAPMARKER.NONE;
							}

							m_StartX = tmpX;
							m_StartY = tmpY;
							m_Map[m_StartX, m_StartY] = (int) MAPMARKER.START;
						}
					}

					if (m_End)
					{
						if (m_Map[tmpX, tmpY] == (int) MAPMARKER.NONE)
						{
							if (m_EndX >= 0)
							{
								m_Map[m_EndX, m_EndY] = (int) MAPMARKER.NONE;
							}

							m_EndX = tmpX;
							m_EndY = tmpY;
							m_Map[m_EndX, m_EndY] = (int) MAPMARKER.END;
						}
					}

					if (m_Obstacle)
					{
						if (m_Map[tmpX, tmpY] == (int) MAPMARKER.NONE)
						{
							m_Map[tmpX, tmpY] = (int) MAPMARKER.OBSTACLE;
						}
					}

					if (m_Clear)
					{
						if (m_Map[tmpX, tmpY] != (int) MAPMARKER.NONE)
						{
							switch (m_Map[tmpX, tmpY])
							{
								case (int) MAPMARKER.START:
									m_StartX = -1;
									m_StartY = -1;
									m_Map[tmpX, tmpY] = (int) MAPMARKER.NONE;
									break;
								case (int) MAPMARKER.END:
									m_EndX = -1;
									m_EndY = -1;
									m_Map[tmpX, tmpY] = (int) MAPMARKER.NONE;
									break;
								case (int) MAPMARKER.OBSTACLE:
									m_Map[tmpX, tmpY] = (int) MAPMARKER.NONE;
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

}
