using UnityEngine;
using System.Collections;

public class GameMonitor : MonoBehaviour
{
    public CheckPoint checkPoint;

    private BoxCollider2D myCollider;
    private bool activated = false;
    // Use this for initialization
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (!activated)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + myCollider.offset, myCollider.size, 0, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Player"));
            if (hits.Length > 0)
            {
                activated = true;
                ActorManager.instance.PlaySound("MonitorActivate", 1f);
                GetComponent<Animator>().Play("MonitorActivate");
                if (ActorManager.instance.checkPoint.checkPointNumber < checkPoint.checkPointNumber)
                {
                    ActorManager.instance.checkPoint = checkPoint;
                    if (checkPoint.checkPointNumber == 5)
                    {
                        ActorManager.instance.PlayLoopingSound("breakthetarget");
                    }
                    else if (ActorManager.instance.GetComponent<AudioSource>().clip.name != "LevelMusic")
                    {
                        ActorManager.instance.PlayLoopingSound("LevelMusic");
                    }
                }
            }
        }
    }
}
