using UnityEngine;

public class SceneScaffolder : MonoBehaviour
{
    public float roomHalfSize = 10.0f;  // 생성할 룸의 절반 크기.
    public float wallHeight = 3.0f; // 벽의 높이.
    public float obstacleSpacing = 3.0f;    // 장애물의 간격.

    public int obstacleRows = 3;
    public int obstacleCols = 3;

    public Vector3 obstacleSize = new Vector3(1.0f, 2.0f, 1.0f);

    private Transform rootParent;   // 부모 트랜스폼.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateRootParent();
        CreateFloor();
        CreateWalls();
        CreateObstacles();
    }

    void CreateRootParent()
    {
        GameObject parentObj = new GameObject("ScaffoldRoot");
        if(parentObj != null)
        {
            rootParent = parentObj.transform;
            rootParent.position = Vector3.zero; // (0, 0, 0)
        }
    }

    void CreateFloor()
    {
        // 바닥 역할을 할 판 오브젝트 생성.
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        if(floor != null)
        {
            floor.name = "Floor";
            floor.transform.SetParent(rootParent);
            floor.transform.position = Vector3.zero;
            float scale = (roomHalfSize * 2.0f) / 10.0f;
            floor.transform.localScale = new Vector3(scale, 1.0f, scale);
        }
    }

    void CreateWalls()
    {
        float size = roomHalfSize * 2.0f;
        float thickness = 0.5f;

        CreateWall("Wall_North", new Vector3(0.0f, wallHeight * 0.5f, roomHalfSize),
            new Vector3(size, wallHeight, thickness));
        CreateWall("Wall_South", new Vector3(0.0f, wallHeight * 0.5f, -roomHalfSize),
            new Vector3(size, wallHeight, thickness));
        CreateWall("Wall_East", new Vector3(roomHalfSize, wallHeight * 0.5f, 0.0f),
            new Vector3(thickness, wallHeight, size));
        CreateWall("Wall_West", new Vector3(-roomHalfSize, wallHeight * 0.5f, 0.0f),
            new Vector3(thickness, wallHeight, size));
    }

    void CreateWall(string name, Vector3 position, Vector3 size)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if(wall != null)
        {
            wall.name = name;
            wall.transform.SetParent(rootParent);
            wall.transform.position = position;
            wall.transform.localScale = size;
        }
    }

    void CreateObstacles()
    {
        if (obstacleRows <= 0 || obstacleCols <= 0)
        {
            return;
        }

        float totalWidth = (obstacleCols - 1) * obstacleSpacing;
        float totalDepth = (obstacleRows - 1) * obstacleSpacing;

        float startX = -totalWidth * 0.5f;
        float startZ = -totalDepth * 0.5f;

        for (int i = 0; i < obstacleRows; ++i)
        {
            for (int j = 0; j < obstacleCols; ++j)
            {
                Vector3 pos = new Vector3(startX + j * obstacleSpacing,
                    obstacleSize.y * 0.5f, startZ + i * obstacleSpacing);
                CreateObstacle("Obstacle_" + i + "_" + j, pos, obstacleSize);
            }
        }
    }

    void CreateObstacle(string name, Vector3 position, Vector3 size)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if(cube != null)
        {
            cube.name = name;
            cube.transform.SetParent(rootParent);
            cube.transform.position = position;
            cube.transform.localScale = size;
        }
    }
}
