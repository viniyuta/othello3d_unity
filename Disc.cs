using Unity.Netcode;
using UnityEngine;

public class Disc : NetworkBehaviour
{
    [SerializeField]
    private Player upColor;
    private Animator animator;

    private void Awake()
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
