using UnityEngine;

public class ScrollingTrees : MonoBehaviour
{

    private Transform cameraTransform;
    private Transform[] layers;
    private float viewZone = 12;
    private int leftIndex;
    private int rightIndex;

    public float backGroundSize;

    // Use this for initialization
    void Start()
    {
        cameraTransform = Camera.main.transform;
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }

    private void ScrollLeft()
    {
        int lastRight = rightIndex;
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backGroundSize);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    private void ScrollRight()
    {
        int lastRight = leftIndex;
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x + backGroundSize);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone))
        {
            ScrollLeft();
        }
        if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - viewZone))
        {
            ScrollRight();
        }
    }
}
