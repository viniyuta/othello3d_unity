using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField]
    private Player upColor;
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Flip()
    {
        if (upColor == Player.Black)
        {
            animator.Play("BlackToWhite");
            upColor = Player.White;
        }
        else 
        {
            animator.Play("WhiteToBlack");
            upColor = Player.Black;
        }
    }

    public void Twitch()
    {
        animator.Play("TwitchDisc");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
