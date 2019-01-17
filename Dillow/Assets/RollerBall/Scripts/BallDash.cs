using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallBody))]
public class BallDash : MonoBehaviour
{

    [HideInInspector] public BallBody body;
    private bool dash_ready = true;
    public float dash_speed;
    public float dash_timer = 1f;
    private Vector3 dash_dir;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<BallBody>();
        body.MoveEvent += OnDash;
        dash_dir = Vector3.zero;
    }

    public void OnDash(bool move, Vector3 dir, int jump, int action)
    {
        if (action == 2 && move && dash_ready && body.CheckPriority(2))
        {
            Debug.Log(Vector3.Angle(dir, body.rb.velocity.normalized));
            dash_dir = (Vector3.Angle(dir, body.rb.velocity.normalized) < 90f) 
                ? (dir + body.rb.velocity.normalized) / 2: dir;

            if (body.lock_enemy)
            {
                Vector3 lock_dir = (body.transform.position - body.lock_enemy.transform.position).normalized;
                dash_dir = (Vector3.Angle(dash_dir, lock_dir) < 15f) 
                    ? lock_dir : dash_dir;
            }

            Dash();
        }
    }

    private void Dash()
    {
        body.rb.velocity = dash_dir * dash_speed;
        StartCoroutine(DashRecharge());
    }

    private IEnumerator DashRecharge()
    {
        dash_ready = false;
        yield return new WaitForSeconds(dash_timer);
        dash_ready = true;
    }
}
