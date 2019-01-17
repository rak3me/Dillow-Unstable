using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MoveDel(bool move, Vector3 dir, int jump);


[RequireComponent(typeof(Rigidbody))]
public class Ball_Body : MonoBehaviour
{

    [Header("Rolling")]
    public float move_power = 5f;
    public float max_speed = 25f;

    [Header("Jumping")]
    public float jump_power = 5f;
    public float jump_time = 0.5f;
    private float jump_time_timer;
    [HideInInspector] public float speed_jump_threshold = 10f;
    private readonly float jump_leeway = 0.2f;
    private float jump_leeway_timer;
    private int collision_count;

    private float jump_multiplier = 2f;
    private float fall_multiplier = 2.5f;
    private Vector3 jump_vector;

    public event MoveDel MoveEvent;
    [HideInInspector ]public bool can_move = true;
    private int priority = 0;

    [HideInInspector] public Rigidbody rb;
    [Header("Locking")]
    public GameObject lock_enemy;

    [HideInInspector] public bool jump_ready;
    [HideInInspector] public bool just_jumped;
    [HideInInspector] public bool mid_air;
    [HideInInspector] public bool air_ready;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = max_speed;
        jump_vector = Vector3.up;
        jump_leeway_timer = jump_leeway;
        MoveEvent += OnMove;
        MoveEvent += OnJump;
    }

    #region MOVEMENT
    // Update is called once per frame
    void FixedUpdate()
    {
        OnFall();
    }

    private void OnFall()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fall_multiplier - 1f) * Time.deltaTime;
        }
        if (rb.velocity.y > 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jump_multiplier - 1f) * Time.deltaTime;
        }
    }

    public void Input(bool move, Vector3 dir, int jump)
    {
       if (can_move)
        {
            MoveEvent(move, dir, jump);
        }
    }

    public bool CheckPriority(int priority, bool set = false)
    {
        if (priority > this.priority) {
            if (set)
                this.priority = priority;
            return true;
        }
        return false;
    }


    private void OnMove(bool move, Vector3 dir, int jump)
    {
       if (move)
        {
            rb.AddTorque(new Vector3(dir.z, 0, -dir.x) * move_power);
            if (mid_air)
                if (CheckPriority(1))
                    rb.AddForce(dir * move_power);
        }

        //Calculate jump direction
        if (rb.velocity.magnitude > speed_jump_threshold)
        {
            if (Vector3.Cross(rb.angularVelocity, rb.velocity).y > 0f)
                jump_vector = (-rb.velocity.normalized + Vector3.up * 2f).normalized;
            else
                jump_vector = (rb.velocity.normalized + Vector3.up * 0.5f).normalized;
        }
        else
            jump_vector = Vector3.up;
    }

    private void OnJump(bool move, Vector3 dir, int jump)
    {
        if (jump == 2 && jump_ready && CheckPriority(1))
        {
            rb.AddForce(jump_vector * jump_power, ForceMode.Impulse);
            just_jumped = true;
            jump_ready = false;
        }

        if (mid_air && !air_ready) {
            if (jump == 1 && jump_time_timer > 0f && rb.velocity.y > 0f)
            {
                rb.AddForce(jump_vector * jump_power);
                jump_time_timer -= Time.deltaTime;
            }
            else if (jump == -1)
            {
                air_ready = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].point.y < transform.position.y) {
            if (just_jumped)
            {
                just_jumped = false;
                mid_air = true;
            }
            else if (!jump_ready)
                OnGround();
        }
    }

    private void OnGround()
    {
        jump_ready = true;
        jump_time_timer = jump_time;
        mid_air = false;
        air_ready = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision_count++;
    }

    private void OnCollisionExit(Collision collision)
    {
        collision_count--;
        if (collision_count < 0)
            collision_count = 0;
        if (collision_count == 0)
        {
            mid_air = true;
            if (jump_ready)
                StartCoroutine(JumpLeeway());
        }
    }

    private IEnumerator JumpLeeway()
    {
        while (jump_leeway_timer > 0f && mid_air)
        {
            jump_leeway_timer -= Time.deltaTime;
            yield return null;
        }

        jump_leeway_timer = jump_leeway;
        if (mid_air)
            jump_ready = false;
    }

    #endregion
}
