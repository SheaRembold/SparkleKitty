using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField]
    Color lowColor = Color.white;
    [SerializeField]
    Color highColor = Color.white;
    [SerializeField]
    float speed = 1f;

    Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        renderer.material.color = Color.Lerp(lowColor, highColor, (Mathf.Sin(Time.time * speed) + 1) * 0.5f);
    }
}