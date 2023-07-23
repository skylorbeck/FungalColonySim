using Unity.Mathematics;
using UnityEngine;

public class BackgroundMaster : MonoBehaviour
{
    public BackgroundPiece prefab;
    public int2 size;
    public float2 spacing;

    void Start()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var piece = Instantiate(prefab, transform);
                piece.transform.localPosition = new Vector3(x * spacing.x, y * spacing.y, 0);
            }
        }

        // this.transform.position = new Vector3(-size.x * spacing.x / 2, -size.y * spacing.y / 2, 0);
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
    }
}