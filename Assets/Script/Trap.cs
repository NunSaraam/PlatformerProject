using UnityEngine;
using UnityEngine.SceneManagement;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HeroKnight player = other.GetComponent<HeroKnight>();
            if (player != null)
            {
                if(player.IsInvincible) return;
                player.TriggerGameOver();
            }
        }
    }
    public void Maingo()
    {
        SceneManager.LoadScene("MainMenu");
    }
}