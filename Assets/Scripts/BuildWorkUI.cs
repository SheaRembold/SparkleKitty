using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildWorkUI : MonoBehaviour
{
    [SerializeField]
    public GameObject tool;
    [SerializeField]
    protected ParticleSystem particles;
    [SerializeField]
    protected Transform line;
    [SerializeField]
    protected float tolerance;
    [SerializeField]
    protected float required;
    [SerializeField]
    protected int particleMax;

    protected BuildArea buildArea;
    protected BuildableData buildable;

    protected float amount;
    protected Vector2 currentPos;
    protected Vector2 lastPos;
    protected bool built = false;

    public void Show(BuildArea buildArea, BuildableData buildable)
    {
        this.buildArea = buildArea;
        this.buildable = buildable;
        amount = 0;
        built = false;

        PlacementManager.Instance.enabled = false;
        UIManager.Instance.ShowUI(gameObject);
        tool.SetActive(true);
        particles.gameObject.SetActive(true);
    }

    public void Hide()
    {
        tool.SetActive(false);
        particles.gameObject.SetActive(false);
        PlacementManager.Instance.enabled = true;
        UIManager.Instance.GoBack();
    }

    private void Update()
    {
        if (built)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            lastPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
            HandleInput();
            lastPos = currentPos;
            int numParts = Mathf.Clamp((int)(amount / required * particleMax), 0, particleMax);
            if (particles.particleCount < numParts)
                particles.Emit(numParts - particles.particleCount);
            if (amount >= required)
            {
                particles.gameObject.SetActive(false);
                buildArea.Build(buildable);
                built = true;
            }
        }
    }

    protected virtual void HandleInput()
    {
        if (currentPos.y <= line.position.y + tolerance && currentPos.y >= line.position.y - tolerance
            && lastPos.y <= line.position.y + tolerance && lastPos.y >= line.position.y - tolerance)
        {
            amount += Mathf.Abs(currentPos.x - lastPos.x);
        }
    }
}