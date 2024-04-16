using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Dictionary<CursorType, GameObject> Cursors = new Dictionary<CursorType, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        updateCursor(Settings.cursor);
    }

    // Update is called once per frame
    public void updateCursor(CursorType type)
    {
        foreach (GameObject cursor in Cursors.Values)
        {
            cursor.SetActive(false);
        }
        Cursors[type].SetActive(true);
    }
}
