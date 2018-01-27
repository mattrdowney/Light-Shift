using UnityEngine;

public static class Circle
{
    public const int sectors = 1*2*3*(4/2)*5*(6/2/3);

    public static Mesh circle_mesh()
    {
        if (circle == null)
        {
            circle = new Mesh();
            Vector3[] vertex_list = new Vector3[sectors*3];
            Vector2[] uv_list = new Vector2[sectors*3];
            int[] triangle_list = new int[sectors*3];
        
            for (int sector = 0; sector < sectors; ++sector)
            {
                float sweep_start = -Mathf.PI*2*(sector/(float)sectors); // negative for clockwise winding of triangles
                float sweep_end = -Mathf.PI*2*((sector+1)/(float)sectors);

                vertex_list[sector*3 + 0] = Vector3.zero; // All sectors include (0, 0).
                vertex_list[sector*3 + 1] = new Vector3(Mathf.Cos(sweep_start), Mathf.Sin(sweep_start), 0);
                vertex_list[sector*3 + 2] = new Vector3(Mathf.Cos(sweep_end), Mathf.Sin(sweep_end), 0);
            
                triangle_list[sector*3 + 0] = sector*3 + 0; // sector points match vertex
                triangle_list[sector*3 + 1] = sector*3 + 1;
                triangle_list[sector*3 + 2] = sector*3 + 2;

                uv_list[sector*3 + 0] = new Vector2((sector+0.5f)/sectors,0.5f); // all sectors points use the same color
                uv_list[sector*3 + 1] = new Vector2((sector+0.5f)/sectors,0.5f);
                uv_list[sector*3 + 2] = new Vector2((sector+0.5f)/sectors,0.5f);
            }
            circle.vertices = vertex_list;
            circle.uv = uv_list;
            circle.triangles = triangle_list;
        }
        return circle;
    }

    private static Mesh circle = null;
}
