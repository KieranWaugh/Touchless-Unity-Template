using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class Cursor : MonoBehaviour
{
    [SerializeField] protected InteractionManager interactionManager;
    [SerializeField] private Camera cam;
    public CursorType cursorType;
    public GameObject InteractionPoint;
    protected Collider2D collider;
    protected CursorManager cursorManager;

    protected bool gestureDetector;

    void OnEnable()
    {
        interactionManager = GameObject.Find("Service Provider").GetComponent<InteractionManager>();
        cursorManager = GameObject.Find("Cursor").GetComponent<CursorManager>();
        interactionManager.CursorUpdate += CursorUpdate;
        if (interactionManager == null)
        {
            SceneManager.LoadScene(0);
        }

        collider = gameObject.GetComponentInChildren<CircleCollider2D>();
        collider.GetComponent<CircleCollider2D>().radius = InteractionPoint.GetComponent<RectTransform>().sizeDelta[0] / 2;

    }

    

    public void CursorUpdate(screenHand positions, Frame frame)
    {
        updatePositions(positions, frame);
        gestureDetector = interactionManager.gesture;
        if (gestureDetector && !interactionManager.settings.activeInHierarchy)
        {
            activateGesature();
        }
        else
        {
            deactivateGesature();
        }
    }

    public abstract void updatePositions(screenHand positions, Frame frame);
    public abstract void activateGesature();
    public abstract void deactivateGesature();

}
