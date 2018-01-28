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
        colors = new Color[Circle.sectors];
         
        mesh_renderer.material = material;
        mesh_renderer.sortingOrder = 0;
        generate_material();
    }

    private void Update()
    {
        if (this == entangled_characters[0])
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
    }

    private void OnTriggerEnter2D()
    {
        Debug.LogError("YESH!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    private void generate_material()
    {
        int number_of_colors = Utility.count_true_booleans(rainbow_keys);
        Color[] color_list;
        if (number_of_colors == 0)
        {
            color_list = new Color[]{ Color.black };
        }
        else
        {
            color_list = new Color[number_of_colors];
            int sublist_index = 0;
            for (int color = 0; color < rainbow_keys.Length; ++color)
            {
                if (rainbow_keys[color])
                {
                    color_list[sublist_index] = rainbow_colors[color];
                    ++sublist_index;
                }
            }
        }
        material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = get_colors(color_list);
        mesh_renderer.material = material;
    }

    public static Texture get_colors(Color[] color_list)
    {
        int block_size = colors.Length / color_list.Length;

        int filled_colors = 0;
        for (int color = 0; color < color_list.Length; ++color)
        {
            for (int range_index = filled_colors * block_size; range_index < (filled_colors + 1)* block_size; ++range_index)
            {
                colors[range_index] = color_list[color];
            }
            ++filled_colors;
        }
        
        Texture2D texture_2d = new Texture2D(Circle.sectors, 1);
        texture_2d.SetPixels(colors);
        texture_2d.Apply();
        return texture_2d;
    }

    public static void swap_colors(int color)
    {
        bool temporary_color_state = entangled_characters[0].rainbow_keys[color];
        entangled_characters[0].rainbow_keys[color] = entangled_characters[1].rainbow_keys[color];
        entangled_characters[1].rainbow_keys[color] = temporary_color_state;
    }

    private Vector2 magnetize()
    {
        Vector2 force = Vector2.zero;
        foreach (Vector2 direction in new Vector2[]{ Vector2.up, Vector2.left, Vector2.down, Vector2.right })
        {
            RaycastHit2D raycast_information = Physics2D.Raycast(transform.position, direction);
            for (int color = 0; color < rainbow_colors.Length; ++color)
            {
                if (raycast_information.collider && rainbow_keys[color])
                {
                    Vector2 color_polarity1 = rainbow_directions[color];
                    Vector2 color_polarity2 = rainbow_directions[raycast_information.transform.gameObject.layer - LayerMask.NameToLayer("Red")];
                    float similarity = Vector2.Dot(color_polarity1, color_polarity2);
                    force -= similarity * direction / raycast_information.distance;
                    Debug.LogWarning("Happening " + Time.time);
                }
            }
        }
        Debug.LogWarning(force.ToString("F6") + " " + Time.time);
        if (Mathf.Abs(force.x) - Mathf.Abs(force.y) > threshold)
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
        else if (Mathf.Abs(force.x) - Mathf.Abs(force.y) < threshold)
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
        RaycastHit2D raycast_information = Physics2D.Raycast(transform.position, direction);
        if (!raycast_information.collider)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            transform.position += (raycast_information.distance - transform.localScale.x)*(Vector3)direction;
        }
    }

    public static void light_shift(int color)
    {
        swap_colors(color);

        entangled_characters[0].generate_material();
        entangled_characters[1].generate_material();

        entangled_characters[0].move();
        entangled_characters[1].move();
    }

    public bool[] rainbow_keys = new bool[rainbow_colors.Length];
    private static Color[] colors = new Color[Circle.sectors];

    private MeshRenderer mesh_renderer;
    private Material material;
    
    private const float threshold = 1e-6f;
    public static readonly Color[] rainbow_colors = new Color[] { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta };
    private static readonly Vector2[] rainbow_directions = new Vector2[] {
            Vector2.right,
            new Vector2(Mathf.Cos(2*Mathf.PI*(1f/6)), Mathf.Sin(2*Mathf.PI*(1f/6))),
            new Vector2(Mathf.Cos(2*Mathf.PI*(2f/6)), Mathf.Sin(2*Mathf.PI*(2f/6))),
            Vector2.left,
            new Vector2(Mathf.Cos(2*Mathf.PI*(4f/6)), Mathf.Sin(2*Mathf.PI*(4f/6))),
            new Vector2(Mathf.Cos(2*Mathf.PI*(5f/6)), Mathf.Sin(2*Mathf.PI*(5f/6))) };
    private static Character[] entangled_characters;
}