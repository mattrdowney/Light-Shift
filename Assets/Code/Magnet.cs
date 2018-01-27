using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
	void Start ()
    {
        if (rainbow_materials == null)
        {
            rainbow_materials = new Material[Circle.sectors];
		    for (int color = 0; color < Character.rainbow_colors.Length; ++color)
            {
                Material material = new Material(Shader.Find("Unlit/Texture"));
                material.mainTexture = Character.get_colors(new Color[] { Character.rainbow_colors[color] });
                rainbow_materials[color] = material;
            }
        }

        MeshFilter mesh_filter = this.GetComponent<MeshFilter>();
        mesh_filter.sharedMesh = Circle.circle_mesh();
        MeshRenderer mesh_renderer = this.GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterial = rainbow_materials[this.gameObject.layer - LayerMask.NameToLayer("Red")];
	}

    public static Material[] rainbow_materials;
}
