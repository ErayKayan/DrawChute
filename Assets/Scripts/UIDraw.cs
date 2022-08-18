using System.Collections.Generic;
using UnityEngine;
using ProBuilder2.Common;

public class UIDraw : MonoBehaviour
{
	#region //Variables
	private TouchInput touch;
	private List<Vector2> touchPositions = new List<Vector2>();
	private bool isPointerOnCanvas = false;

	public UILineTextureRenderer linerenderer;
	public float twoPointMaxDis = 10f;

	public Material material;
	[HideInInspector] public GameObject currentDrawObject;
	#endregion
	#region //Unity Methods
	private void Start()
	{
		touch = GetComponent<TouchInput>();
	}

	private void Update()
	{
		if (isPointerOnCanvas)
		{
			if (touch.isEvent)
			{
				if (Input.GetKeyDown(KeyCode.Mouse0))
				{
					FirstLinePoint();
				}
				if (Input.GetKey(KeyCode.Mouse0))
				{
					UpdateLine();
				}
			}
			else
			{
				if (Input.GetKeyUp(KeyCode.Mouse0))
				{
					if (touchPositions.Count > 5)
					{
						CreateObject(touchPositions);
					}
					else
					{
						Time.timeScale = 1;
					}
				}
			}
		}
	}
	#endregion
	#region //Methods

	public void SetIsPointerOnCanvas(bool value) //Event Trigger checks pointer
	{
		isPointerOnCanvas = value;
	}

	private void FirstLinePoint()
	{
		Time.timeScale = 0.3f;
		touchPositions.Clear();
		Vector2 firstPosition = touch.startPosition.InversePoint(linerenderer.transform);
		touchPositions.Add(firstPosition);
		linerenderer.Points = touchPositions.ToArray();
	}

	private void UpdateLine()
	{
		Vector2 touchPos = touch.currentPosition.InversePoint(linerenderer.transform);
		float distance = Vector2.Distance(touchPositions[touchPositions.Count - 1], touchPos);
		if (distance > twoPointMaxDis)
		{
			touchPositions.Add(touchPos);
			linerenderer.Points = touchPositions.ToArray();
		}
	}

	public void CreateObject(List<Vector2> points)
	{
		GameObject createObject = new GameObject();
		var newObject = createObject.AddComponent<pb_Object>();
		createObject.transform.Translate(0, 3, 0);
		createObject.AddComponent<pb_Entity>();
		pb_BezierShape pathObject = createObject.AddComponent<pb_BezierShape>();
		List<pb_BezierPoint> beizers = BeizerListCreate(points);
		pathObject.m_Points = beizers;
		pathObject.m_Radius = 25;
		pathObject.Refresh();
		createObject.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
		newObject.ToMesh();
		Mesh m = createObject.GetComponent<MeshFilter>().sharedMesh;
		MeshRenderer mr = createObject.GetComponent<MeshRenderer>();
		MeshCollider mc = createObject.AddComponent<MeshCollider>();
		mc.sharedMesh = m;
		mc.convex = true;
		mr.sharedMaterial = material;

		NewDrawObjectPlace(createObject);

		Time.timeScale = 1;
	}

	private List<pb_BezierPoint> BeizerListCreate(List<Vector2> points)
	{
		List<pb_BezierPoint> beizerPoints = new List<pb_BezierPoint>();
		for (int i = 0; i < points.Count; i++)
		{
			beizerPoints.Add(new pb_BezierPoint(points[i], points[i], points[i], Quaternion.identity));
		}
		return beizerPoints;
	}

	private void NewDrawObjectPlace(GameObject oObject)
	{
		if (currentDrawObject != null)
			Destroy(currentDrawObject);
		currentDrawObject = oObject;
	}

	public void CreateWhenPointerExit()
	{
		CreateObject(touchPositions);
	}
	
	#endregion
}
