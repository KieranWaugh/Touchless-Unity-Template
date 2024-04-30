using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Leap.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Widget : MonoBehaviour
{

	public enum Type
	{
		Button,
		Slider, 
		ButtonRound
	}

	[SerializeField] protected Color Color = Color.red;
	public Type type;
	public Shape shape;
	protected bool focussed;
	protected bool active;
	protected float distance;
	protected float width, height;
	protected bool target = false;
	protected bool widgetActive = true;
	protected InteractionManager interactionManager;
	public Action<GameObject> OnFocus, OnUnFocus, OnActivate, OnUnActivate;
	protected PinchDetector pinchDetector;
	protected GameObject cursor;
	protected Vector2 prevPosition;
	protected bool overlap = false;


    private void Awake()
	{
		interactionManager = GameObject.Find("Service Provider").GetComponent<InteractionManager>();

		if (interactionManager == null)
		{
			SceneManager.LoadScene(0);
		}
		
	}

	// Start is called before the first frame update
	void Start()
	{

		width = gameObject.GetComponent<RectTransform>().sizeDelta.x;
		height = gameObject.GetComponent<RectTransform>().sizeDelta.y;
		interactionManager.addWidget(gameObject);
		
		prevPosition = Vector2.zero;
		SetColour(Color);


    }

	// called once per frame
	protected void Update()
	{
		cursor = interactionManager.cursor;
		if (widgetActive){
			distance = getCursorDistance();
		}else{
			distance = -1;
		}
        
        

    }

	public float getDistance()
	{
		return distance;
	}

	public bool isTarget()
	{
		return target;
	}

	public void SetWidgetActive(bool flag){
		if(flag){
			widgetActive = true;
			SetColour(Color);
		}
		else{
			widgetActive = false;
			SetColour(Color.gray);
		}
	}

	public abstract float getCursorDistance();

	public abstract void setFocus(bool flag);

	public abstract void activate(bool down, bool up, bool held, bool first);

	public abstract void setTarget(bool flag);

	public abstract void SetColour(Color color);



}

public enum Shape{
	Circle, 
	Rectangle
}
