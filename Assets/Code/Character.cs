using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private void Start()
    {
        MeshFilter mesh_filter = this.gameObject.GetComponent<MeshFilter>();
        mesh_filter.sharedMesh = Circle.circle_mesh();
        
        mesh_renderer = this.GetComponent<MeshRenderer>();
        material = new Material(Shader.Find("Unlit/Texture"));
        texture_2d = new Texture2D(Circle.sectors, 1);
        colors = new Color[Circle.sectors];
         
        mesh_renderer.material = material;
        mesh_renderer.sortingOrder = +1000;
        recalculate_colors();
    }

    private void recalculate_colors()
    {
        int number_of_colors = Utility.count_true_booleans(rainbow_keys);
        int block_size = (number_of_colors > 0 ? colors.Length / number_of_colors : 0);

        for (int color = 0; color < colors.Length; ++color) // Reset all colors so that the circle is black by default.
        {
            colors[color] = Color.black;
        }

        int filled_colors = 0;
        for (int color = 0; color < rainbow_colors.Length; ++color)
        {
            if (rainbow_keys[color])
            {
                for (int range_index = filled_colors*block_size; range_index < (filled_colors+1)*block_size; ++range_index)
                {
                    colors[range_index] = rainbow_colors[color];
                }
                ++filled_colors;
            }
        }

        texture_2d.SetPixels(colors);
        texture_2d.Apply();
        material.mainTexture = texture_2d;
        mesh_renderer.material = material;
    }

    public bool[] rainbow_keys = new bool[rainbow_colors.Length];
    private Color[] colors = new Color[Circle.sectors];

    private MeshRenderer mesh_renderer;
    private Material material;
    private Texture2D texture_2d;

    private static readonly Color orange = new Color(255f/255f, 127f/255f, 0);
    private static readonly Color indigo = new Color(075f/255f, 0, 130f/255f);
    private static readonly Color violet = new Color(128f/255f, 0, 255f/255f);
    private static readonly Color[] rainbow_colors = new Color[] { Color.red, orange, Color.yellow, Color.green, Color.blue, indigo, violet };

}