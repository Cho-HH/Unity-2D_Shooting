using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] mapBottom;
    [SerializeField] GameObject[] mapMiddle;
    [SerializeField] GameObject[] mapTop;

    float viewHeight;
    LinkedList<GameObject> bottomList = new LinkedList<GameObject>();
    LinkedList<GameObject> middieList = new LinkedList<GameObject>();
    LinkedList<GameObject> topList = new LinkedList<GameObject>();
    void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2 + 5;
        for (int i = 0; i < 3; i++)
        {
            bottomList.AddLast(mapBottom[i]);
            middieList.AddLast(mapMiddle[i]);
            topList.AddLast(mapTop[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject map = null;
        Vector3 lastPos = Vector3.zero;

        if (bottomList.First.Value.transform.position.y < -viewHeight)
        {
            map = bottomList.First.Value;
            bottomList.RemoveFirst();
            lastPos = bottomList.Last.Value.transform.position;
            bottomList.AddLast(map);

            map.transform.position = new Vector2(lastPos.x, lastPos.y + 12);
        }

        if (middieList.First.Value.transform.position.y < -viewHeight)
        {
            map = middieList.First.Value;
            middieList.RemoveFirst();
            lastPos = middieList.Last.Value.transform.position;
            middieList.AddLast(map);

            map.transform.position = new Vector2(lastPos.x, lastPos.y + 12);
        }

        if (topList.First.Value.transform.position.y < -viewHeight)
        {
            map = topList.First.Value;
            topList.RemoveFirst();
            lastPos = topList.Last.Value.transform.position;
            topList.AddLast(map);

            map.transform.position = new Vector2(lastPos.x, lastPos.y + 12);
        }
    }
}
