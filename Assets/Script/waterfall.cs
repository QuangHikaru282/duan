using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class WaterfallDisappearance : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        // Lấy Tilemap từ GameObject
        tilemap = GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("Tilemap không được gắn vào GameObject này.");
        }
    }

    public void StartDisappearingEffect()
    {
        StartCoroutine(DisappearTiles());
    }

    IEnumerator DisappearTiles()
    {
        if (tilemap == null)
            yield break;

        // Lấy giới hạn kích thước của Tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Vòng lặp qua các hàng của Tilemap, từ dưới lên trên
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // Kiểm tra nếu ô tile tại vị trí này tồn tại
                if (tilemap.HasTile(tilePosition))
                {
                    // Xóa tile tại vị trí này
                    tilemap.SetTile(tilePosition, null);
                }
            }

            // Chờ một khoảng thời gian giữa mỗi hàng
            yield return new WaitForSeconds(0.2f); // Thời gian giữa các hàng
        }

        // Không destroy GameObject theo yêu cầu của bạn
        // Nếu bạn muốn tắt Waterfall, sử dụng gameObject.SetActive(false);
    }
    IEnumerator FadeOutMaterial()
    {
        TilemapRenderer renderer = GetComponent<TilemapRenderer>();
        Material mat = renderer.material;

        float duration = 2f; // Thời gian mờ dần
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / duration;
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
            yield return null;
        }
    }

}
