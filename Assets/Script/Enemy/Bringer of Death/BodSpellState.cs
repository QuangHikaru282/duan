using UnityEngine;
using System.Collections;

public class BodSpellState : State
{
    [Header("Spell Settings")]
    public GameObject darkHandPrefab;
    public AnimationClip bodCastAnim;
    public int darkHandCount = 3;
    [SerializeField] private float spawnDelay = 0.3f;
    [SerializeField] private float horizontalDriftStep = 0.5f;

    [Header("Cooldown Settings")]
    [SerializeField] private float spellCooldown = 6f;
    private float lastSpellTime = -Mathf.Infinity;

    private Vector3 castPosition;
    private float capturedVelocityX;
    private bool hasSpawned = false;
    private float animDuration = 0.8f;
    private Coroutine spawnRoutine;

    public override int priority => 50;

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;
        hasSpawned = false;

        body.velocity = Vector2.zero;

        if (bodCastAnim != null && animator != null)
            animator.Play(bodCastAnim.name);

        var playerScript = GameObject.FindGameObjectWithTag("Player")?.GetComponent<playerScript>();
        if (playerScript != null)
        {
            castPosition = playerScript.transform.position;
            capturedVelocityX = playerScript.GetComponent<Rigidbody2D>()?.velocity.x ?? 0f;
        }
    }

    public override void Do()
    {
        if (!hasSpawned && darkHandPrefab != null)
        {
            spawnRoutine = core.StartCoroutine(SpawnDarkHands());
        }

        if (time >= animDuration)
        {
            lastSpellTime = Time.time;
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public override void Exit()
    {
        if (spawnRoutine != null)
        {
            core.StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        if (!isComplete)
        {
            lastSpellTime = Time.time; // Đánh dấu cooldown như đã dùng
            exitReason = StateExitReason.NormalComplete;
        }
    }


    public override State GetNextState()
    {
        if (!isComplete || exitReason != StateExitReason.NormalComplete) return null;
        return los.isSeeingTarget ? core.idleState : core.searchState;
    }

    public bool IsSpellReady()
    {
        return Time.time >= lastSpellTime + spellCooldown;
    }

    private IEnumerator SpawnDarkHands()
    {

        hasSpawned = true;
        float driftDir = Mathf.Sign(capturedVelocityX);

        Vector3 basePos = castPosition + new Vector3(0f, 0.7f, 0f); // spawn từ trên đầu player (có thể tinh chỉnh thêm 1.5f nếu cần)

        for (int i = 0; i < darkHandCount; i++)
        {
            float xOffset = i * horizontalDriftStep * driftDir;
            float yRandom = Random.Range(-0.2f, 0.2f); // random nhẹ theo trục Y để tránh đều tăm tắp

            Vector3 spawnPos = basePos + new Vector3(xOffset, yRandom, 0f);
            Instantiate(darkHandPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);

        }
    }

}
