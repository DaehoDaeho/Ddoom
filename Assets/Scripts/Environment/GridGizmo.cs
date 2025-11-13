using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GridGizmo : MonoBehaviour
{
    public int gridSize = 10;
    public float cellSize = 1.0f;

    public Vector3 offset = Vector3.zero;
    public bool enabledInPlayMode = true;

    private void OnDrawGizmos()
    {
        if(Application.isPlaying == true && enabledInPlayMode == false)
        {
            return;
        }

        if(gridSize <= 0 || cellSize <= 0.0f)
        {
            return;
        }

        Gizmos.color = Color.grey;

        float size = gridSize * cellSize;

        for(int x = 0; x <= gridSize; ++x)
        {
            float xPos = (-gridSize * 0.5f) + x * cellSize;
            Vector3 from = transform.position + offset + new Vector3(xPos, 0.0f, -size * 0.5f);
            Vector3 to = transform.position + offset + new Vector3(xPos, 0.0f, size * 0.5f);
            Gizmos.DrawLine(from, to);
        }

        for (int z = 0; z <= gridSize; ++z)
        {
            float zPos = (-gridSize * 0.5f) + z * cellSize;
            Vector3 from = transform.position + offset + new Vector3((-gridSize * 0.5f), 0.0f, zPos);
            Vector3 to = transform.position + offset + new Vector3(size * 0.5f, 0.0f, zPos);
            Gizmos.DrawLine(from, to);
        }
    }
}
