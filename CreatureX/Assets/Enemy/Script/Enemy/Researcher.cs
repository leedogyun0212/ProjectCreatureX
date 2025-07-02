using UnityEngine;

public class Researcher : BaseEnemy
{

    public float helpRange = 10f;

    private bool isHelp = true;

    public void RequestHelp()
    {
        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, helpRange, LayerMask.GetMask("Enemy"));
        foreach (var ally in allies)
        {
            IAllyResponder responder = ally.GetComponent<IAllyResponder>();
            if (responder != null)
            {
                responder.OnHelpRequested(this.transform);
            }
        }
    }

    public void AnimHelpStop()
    {
        animWalk = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !player.isAttack&&isHelp)
        {
            monsterState = MonsterState.Play;
            animWalk = 2;
            RequestHelp();
            isHelp = false;

        }
    }
}
