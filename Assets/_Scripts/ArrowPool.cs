using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    public GameObject arrowPrefab;
    public int maxArrows = 9;
    public float arrowLifeTime = 10f;

    private Queue<Arrow> arrows;
    private List<Arrow> activeArrows;

    void Awake()
    {
        Instance = this;
        arrows = new Queue<Arrow>(maxArrows);
        activeArrows = new List<Arrow>(maxArrows);

        for (int i = 0; i < maxArrows; i++)
        {
            Arrow arrow = Instantiate(arrowPrefab, transform).GetComponent<Arrow>();
            arrow.gameObject.SetActive(false);
            arrows.Enqueue(arrow);
        }
    }

    public Arrow GetArrow()
    {
        if (arrows.Count > 0)
        {
            Arrow arrow = arrows.Dequeue();
            arrow.gameObject.SetActive(true);
            activeArrows.Add(arrow);
            return arrow;
        }
        else
        {
            Arrow oldestArrow = activeArrows[0];
            oldestArrow.gameObject.SetActive(false);
            activeArrows.RemoveAt(0);
            arrows.Enqueue(oldestArrow);

            Arrow newArrow = arrows.Dequeue();
            newArrow.gameObject.SetActive(true);
            activeArrows.Add(newArrow);
            return newArrow;
        }
    }

    public void ReturnToPool(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
        arrows.Enqueue(arrow);
        activeArrows.Remove(arrow);
    }
}
