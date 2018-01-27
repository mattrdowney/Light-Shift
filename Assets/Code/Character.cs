using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    private void Start()
    {
        if (entangled_characters == null)
        {
            entangled_characters = GameObject.FindObjectsOfType<Character>();
        }

        MeshFilter mesh_filter = this.gameObject.GetComponent<MeshFilter>();
        mesh_filter.sharedMesh = Circle.circle_mesh();
        
        mesh_renderer = this.GetComponent<MeshRenderer>();
        material = new Material(Shader.Find("Unlit/Texture"));
        texture_2d = new Texture2D(Circle.sectors, 1);
        colors = new Color[Circle.sectors];
         
        mesh_renderer.material = material;
        mesh_renderer.sortingOrder = 0;
        recalculate_colors();
    }

    private void Update()
    {
        for (int color = 0; color < rainbow_colors.Length; ++color)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + color))
            {
                light_shift(color);
                break;
            }
        }
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

    private static void swap_colors(int color)
    {
        bool temporary_color_state = entangled_characters[0].rainbow_keys[color];
        entangled_characters[0].rainbow_keys[color] = entangled_characters[1].rainbow_keys[color];
        entangled_characters[1].rainbow_keys[color] = temporary_color_state;
    }

    private Vector2 magnetize()
    {
        Vector2 force = Vector2.zero;
        foreach (Vector2 direction in new Vector2[]{ Vector2.left, Vector2.right, Vector2.down, Vector2.up })
        {
            RaycastHit2D raycast_information = Physics2D.Raycast(transform.position, direction);
            for (int color = 0; color < rainbow_colors.Length; ++color)
            {
                if (rainbow_keys[color])
                {
                    force += direction * Vector2.Dot(rainbow_directions[color], rainbow_directions[raycast_information.transform.gameObject.layer - 16]);
                }
            }
        }
        if (Mathf.Abs(force.x) > Mathf.Abs(force.y))
        {
            if (force.x > 0)
            {
                return Vector2.right;
            }
            else if (force.x < 0)
            {
                return Vector2.left;
            }
        }
        else if (Mathf.Abs(force.x) < Mathf.Abs(force.y))
        {
            if (force.y > 0)
            {
                return Vector2.up;
            }
            else if (force.y < 0)
            {
                return Vector2.down;
            }
        }
        return Vector2.zero;
    }

    private void move()
    {
        Vector2 direction = magnetize();
        int layer_mask = ~(1 << LayerMask.NameToLayer("Player"));
        RaycastHit2D raycast_information = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, layer_mask);
        if (!raycast_information.collider)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        transform.position += (raycast_information.distance - transform.localScale.x/2)*(Vector3)direction;
    }

    public static void light_shift(int color)
    {
        swap_colors(color);
        entangled_characters[0].move();
        entangled_characters[1].move();
    }

    public bool[] rainbow_keys = new bool[rainbow_colors.Length];
    private Color[] colors = new Color[Circle.sectors];

    private MeshRenderer mesh_renderer;
    private Material material;
    private Texture2D texture_2d;
    
    private static readonly Color[] rainbow_colors = new Color[] { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta };
    private static readonly Vector2[] rainbow_directions = new Vector2[] {
            Vector2.right,
            new Vector2(Mathf.Cos(2*Mathf.PI*(1/6)), Mathf.Sin(2*Mathf.PI*(1/6))),
            new Vector2(Mathf.Cos(2*Mathf.PI*(2/6)), Mathf.Sin(2*Mathf.PI*(2/6))),
            Vector2.left,
            new Vector2(Mathf.Cos(2*Mathf.PI*(4/6)), Mathf.Sin(2*Mathf.PI*(4/6))),
            new Vector2(Mathf.Cos(2*Mathf.PI*(5/6)), Mathf.Sin(2*Mathf.PI*(5/6))) };
    private static Character[] entangled_characters;
}