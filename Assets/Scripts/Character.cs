﻿using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    //Character stats
    protected float        health = 10;
    protected float        sanity = 10;
    protected float        drunkAmount;
    protected float        stamina = 100;
    protected float        staminaRecoveryRate = 2;

    //Max stats
    protected int          maxHealth = 100;
    protected int          maxSanity = 100;
    protected int          maxStamina= 100;

    //stat decay variables
    protected float healthDecay    = 1/4;
    protected float sanityDecay    = 1/10;
    protected float drunkDecay     = 1/10;
    protected float staminaDecay   = 10;

    //Character movement
    [SerializeField]
    protected float        movementSpeed;
    [SerializeField]
    public float        sprintSpeed;
    public Vector3         movementDirection;
    protected bool         sprinting;

    //Collision variables
    [SerializeField]
    protected int          NoOfRays;
    [SerializeField]
    private LayerMask      raycastMask;
    private float          lengthOfRay;

    //Animation variables
    protected Animator     animator;
    private Sprite         currentIdleSprite = null;
    [SerializeField]
    private Sprite[]       idleSprites;
    private SpriteRenderer Sr;

    //Exhaust variables
    [SerializeField]
    protected bool         exhausted; //Must disable sprinting
    private float          exhaustTimer;
    protected float        exhaustDuration = 5;

    //Inventory variable
    protected Inventory    characterInventory;

    //Get & Set
    public float   Health
    {
        get
        {
            return health;
        }

        set
        {
            if (value <= 0)
            {
                Death();
            }
            else
            {
                health = Mathf.Clamp(value, 0, maxHealth);
            }
        }
    }
    public float   Sanity
    {
        get
        {
            return sanity;
        }

        set
        {
            if (value <= 0)
            {
                Death();
            }
            sanity = Mathf.Clamp(value, 0, maxSanity);
        }
    }
    public float   Stamina
    {
        get
        {
            return stamina;
        }

        set
        {
            if (value <= 0f)
            {
                Exhausted();
                stamina = 0f;
            }
            else
            {
                stamina = Mathf.Clamp(value, 0f, maxStamina);
            }
        }
    }
    public float   DrunkAmount
    {
        get
        {
            return drunkAmount;
        }

        set
        {
            drunkAmount = value;
        }
    }
    public bool Sprinting
    {
        get
        {
            return sprinting;
        }
    }


    protected virtual void ApplyMovement()
    {
        if (sprinting && !exhausted)
        {
            Stamina -= staminaDecay * Time.deltaTime;
            transform.Translate(movementDirection * sprintSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        }

    }

    protected abstract void GetInput();

    protected abstract void Death();
    protected abstract void Attack();
    public abstract void ConsumeItem(int itemID);
    protected abstract void Gather();
    protected abstract void Beg();

    public void TakeHealthDamage(int amount)
    {
        Health -= amount;
    }
    public void TakeSanityDamage(int amount)
    {
        Sanity -= amount;
    }

    protected virtual void RecoverStamina()
    {
        if (!sprinting)
        {
            if (Stamina < maxStamina)
            {
                Stamina += staminaRecoveryRate * Time.deltaTime;
            }
        }
        if(sprinting)
        {
            if(Stamina <= 0)
            {
                Exhausted();
                ExhaustTimer();
            }
        }
    }
    protected virtual void Collision()
    {
        Ray2D ray;
        Vector2 origin;

        //Origin starting determination from characters collider whenever the character is moving horizontally or vertically
        if (movementDirection.y != 0)
        {
            origin = new Vector2(GetComponent<Collider2D>().bounds.min.x, GetComponent<Collider2D>().bounds.center.y);
        }
        else
        {
            origin = new Vector2(GetComponent<Collider2D>().bounds.center.x, (GetComponent<Collider2D>().bounds.min.y));
        }


        //Distance between rays determined by direction
        float distanceBetweenRaysX = (GetComponent<Collider2D>().bounds.size.x) / (NoOfRays - 1);
        float distanceBetweenRaysY = (GetComponent<Collider2D>().bounds.size.y) / (NoOfRays - 1);

        //loop for raycasting
        for (int i = 0; i < NoOfRays; i++)
        {
            ray = new Ray2D(origin, (movementDirection).normalized);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);

            RaycastHit2D BuildingHit = Physics2D.Raycast(ray.origin, ray.direction, lengthOfRay, raycastMask);

            RaycastHit2D LitterHit = Physics2D.Raycast(ray.origin, ray.direction, lengthOfRay, 1 << 8);

            if (BuildingHit)
            {
                movementDirection = movementDirection - Vector3.Project(movementDirection, BuildingHit.normal.normalized);
                return;
            }
            //Checking the litter we hit and adding it to inventory
            if (LitterHit)
            {
                characterInventory.AddItemToInventory(LitterHit.collider.gameObject.GetComponent<Consumable>());
                Destroy(LitterHit.collider.gameObject);
            }

            //Adding new raycast to next point
            if (movementDirection.x != 0 && movementDirection.y == 0)
                origin += new Vector2(0, distanceBetweenRaysY);
            else
                origin += new Vector2(distanceBetweenRaysX, 0);
        }
    }


    protected virtual void Exhausted()
    {
        if (exhausted == false)
        {
            exhausted = true;
        }
    }

    protected void ExhaustTimer()
    {
        //Only tick when exhausted
        if (exhausted)
        {
            //Still exhausted
            if (exhaustTimer > 0f)
            {
                exhaustTimer -= Time.deltaTime;
            }
            //Not exhausted
            else
            {
                exhausted = false;
                exhaustTimer = exhaustDuration;
            }
        }
    }

    protected void AnimationChanger()
    {
        //sideways movement animator changer
        if (movementDirection.x != 0 && movementDirection.y == 0) { animator.Play(AnimationClips.WalkSideways.ToString()); currentIdleSprite = idleSprites[3]; SpriteFlip(); }

        //walking down animator changer
        if (movementDirection.x == 0 && movementDirection.y < 0) { animator.Play(AnimationClips.WalkDown.ToString()); currentIdleSprite = idleSprites[0]; SpriteFlip(); }

        //walking upwards animation changer
        if (movementDirection.x == 0 && movementDirection.y > 0) { animator.Play(AnimationClips.WalkUp.ToString()); currentIdleSprite = idleSprites[4]; SpriteFlip(); }

        //strafing upwards animation changer
        if (movementDirection.x != 0 && movementDirection.y > 0) { animator.Play(AnimationClips.WalkStrafeUp.ToString()); currentIdleSprite = idleSprites[1]; SpriteFlip(); }

        //strafing downwards animation changer
        if (movementDirection.x != 0 && movementDirection.y < 0) { animator.Play(AnimationClips.WalkstrafeDown.ToString()); currentIdleSprite = idleSprites[2]; SpriteFlip(); }

        //Idle
        if (movementDirection.x == 0 && movementDirection.y == 0) { animator.Play(AnimationClips.Idle.ToString()); Sr.sprite = currentIdleSprite; }

    }

    protected void SpriteFlip()
    {
        bool flip;
        flip = movementDirection.x > 0 ? true : false;
        Sr.flipX = flip;
    }

    protected void StatsDecay()
    {
        Health      -= healthDecay * Time.deltaTime;
        Sanity      -= sanityDecay * Time.deltaTime;
        DrunkAmount -= drunkDecay * Time.deltaTime;
    }

    enum AnimationClips
    {
        Idle,
        WalkSideways,
        WalkDown,
        WalkStrafeUp,
        WalkstrafeDown,
        WalkUp
    }
    protected virtual void Awake()
    {

    }
    // Use this for initialization
    protected virtual void Start()
    {
        exhaustTimer = exhaustDuration;
        lengthOfRay = GetComponent<Collider2D>().bounds.extents.magnitude / 2;
        Sr = GetComponent<SpriteRenderer>();
        characterInventory = gameObject.AddComponent<Inventory>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        GetInput();
        StatsDecay();
        RecoverStamina();
        ExhaustTimer();
        Collision();
        AnimationChanger();
        ApplyMovement();
    }
}
