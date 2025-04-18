using UnityEngine;
using UnityEngine.UI;

public class EnemyCore : MonoBehaviour, IEnemy
{
    [Header("Common Components")]
    public Rigidbody2D body;
    public Animator animator;
    public GroundSensor groundSensor;

    [Header("Shared States")]
    public IdleState idleState;
    public PatrolState patrolState;
    public NavigateState navigateState;
    public ChaseState chaseState;
    public SearchState searchState;
    public AttackState attackState;
    public HurtState hurtState;
    public DieState dieState;

    [Header("Player Reference")]
    public Transform player;
    public Vector2 lastKnownPosition;

    [Header("HP Settings")]
    public GameObject hpCanvasObj;
    public Slider hpSlider;
    public int maxHP = 10;
    private int currentHP;
    public float hpTimer = 0f;
    public float hpHideDelay = 5f;
    public bool canBeDamaged = true;

    [Header("Effect Prefabs")]
    public GameObject meleeEffect;
    public GameObject rangeEffect;
    public GameObject floatingTextPrefab;

    [Header("Die Settings")]
    public float fallBelowOffsetY = -100f;

    public StateMachine machine;
    public State state => machine?.state;

    public virtual void Start()
    {
        InitCore();
        SetupInstances();
        machine.Set(idleState);
    }

    protected void InitCore()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        currentHP = maxHP;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if (hpCanvasObj != null)
            hpCanvasObj.SetActive(false);
    }

    public void SetupInstances()
    {
        machine = new StateMachine();
        State[] allStates = GetComponentsInChildren<State>();
        foreach (var st in allStates)
        {
            bool hasSubMachine = st is ChaseState;
            st.SetCore(this, hasSubMachine);
        }
    }

    public virtual void Update()
    {
        if (machine != null && machine.state != null)
        {
            if (machine.state.isComplete)
            {
                var next = machine.state.GetNextState();
                if (next != null)
                    machine.Set(next);
            }

            machine.state.DoBranch();
        }

        if (hpCanvasObj != null && hpCanvasObj.activeSelf)
        {
            hpTimer += Time.deltaTime;
            if (hpTimer >= hpHideDelay)
                HideHPAndReset();
        }

        if (transform.position.y < fallBelowOffsetY && machine != null && state != dieState)
        {
            machine.TrySet(dieState, true);
        }
    }

    void FixedUpdate()
    {
        if (machine != null && machine.state != null)
            machine.state.FixedDoBranch();
    }

    public void TakeDamage(int dmg, string dmgType, int attackDir)
    {
        if (!canBeDamaged || state == dieState) return;

        currentHP -= dmg;
        currentHP = Mathf.Max(currentHP, 0);

        if (hpSlider != null)
            hpSlider.value = currentHP;

        if (hpCanvasObj != null && !hpCanvasObj.activeSelf)
            hpCanvasObj.SetActive(true);

        hpTimer = 0f;

        ShowHitEffect(dmgType, attackDir);
        ShowFloatingText(dmg);

        if (machine == null) return;

        if (currentHP <= 0)
        {
            machine.TrySet(dieState, true);
        }
        else if (state != hurtState)
        {
            machine.TrySet(hurtState, true);
        }
    }

    void ShowHitEffect(string dmgType, int dir)
    {
        GameObject effectPrefab = null;

        if (dmgType == "Melee" && meleeEffect != null)
            effectPrefab = meleeEffect;
        else if (dmgType == "Range" && rangeEffect != null)
            effectPrefab = rangeEffect;

        if (effectPrefab == null) return;

        Vector3 offset = new Vector3(0.3f * dir, 0f, 0f);
        GameObject effect = Instantiate(effectPrefab, transform.position + offset, Quaternion.identity);

        Vector3 scale = effect.transform.localScale;
        scale.x = (dir < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        effect.transform.localScale = scale;

        Destroy(effect, 0.8f); 
    }


    void ShowFloatingText(int dmg)
    {
        if (floatingTextPrefab == null || currentHP <= 0 || hpCanvasObj == null) return;

        GameObject go = Instantiate(floatingTextPrefab, hpCanvasObj.transform, false);
        FloatingText ft = go.GetComponent<FloatingText>();
        if (ft != null)
            ft.SetText($"-{dmg}");
    }

    public void HideHPAndReset()
    {
        currentHP = maxHP;
        if (hpSlider != null)
            hpSlider.value = currentHP;
        if (hpCanvasObj != null)
            hpCanvasObj.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (machine == null || state == dieState)
            return;

        if (other.CompareTag("Trap"))
        {
            machine.TrySet(dieState, true);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || machine == null || state == null) return;

        var activeStates = machine.GetActiveStateBranch();
        string path = "";
        foreach (var s in activeStates)
            path += s.GetType().Name + " > ";

        if (path.Length > 3)
            path = path.Substring(0, path.Length - 3);

        UnityEditor.Handles.Label(transform.position, "Active States: " + path);
    }
#endif
}
