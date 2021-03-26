using MinecraftClone.World;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonDigger : MonoBehaviour
{
    WorldGenerator worldGenerator;
    RawImage renderImage;

    private void Start()
    {
        worldGenerator = WorldGenerator.instance;
        renderImage =  FindObjectOfType<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        bool leftClick = Input.GetMouseButtonDown(0);

        if (leftClick)
        {
            RaycastHit hit;
            Vector2 viewPortPosition = new Vector2(
                Input.mousePosition.x / renderImage.rectTransform.rect.xMax,
                Input.mousePosition.y / renderImage.rectTransform.rect.yMax) / 2;

            Ray mouseRay = Camera.main.ViewportPointToRay(viewPortPosition);

            if (Physics.Raycast(mouseRay, out hit, 12f))
            {
                Vector3 hitPoint = hit.point + transform.forward * .01f;

                int chunkPosX = Mathf.FloorToInt(hitPoint.x / worldGenerator.ChunkDimensions.x);
                int chunkPosZ = Mathf.FloorToInt(hitPoint.z / worldGenerator.ChunkDimensions.z);

                int hitpointX = Mathf.FloorToInt(hitPoint.x) ;
                int hitpointY = Mathf.FloorToInt(hitPoint.y);
                int hitpointZ = Mathf.FloorToInt(hitPoint.z);

                int voxelPosX = mod(hitpointX, worldGenerator.ChunkDimensions.x);
                int voxelPosY = hitpointY;
                int voxelPosZ = mod(hitpointZ, worldGenerator.ChunkDimensions.z);

                worldGenerator.UpdateVoxelData(
                    new Vector3(chunkPosX, 0, chunkPosZ),
                    new Vector3Int(voxelPosX, voxelPosY, voxelPosZ),
                    VoxelType.AIR);
            }
        }
    }

    int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
