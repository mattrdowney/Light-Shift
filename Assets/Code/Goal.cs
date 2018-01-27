using UnityEngine.SceneManagement;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Goal pair;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            goal_met = true;
            if (pair.goal_met)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            goal_met = false;
        }
    }
    
    private bool goal_met = false;
}
