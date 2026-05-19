using System.Collections;
using UnityEngine;

public class PlayerDead : MonoBehaviour // Changed class name from PlayerDeathAnimator to PlayerDead
{
    [Header("Death Animation Settings")]
    [Tooltip("The name of the death animation in the Animator Controller")]
    public string deathAnimationName = "Dead";

    [Tooltip("How long to wait before starting the death effect after animation begins")]
    public float deathEffectDelay = 0.5f;

    [Tooltip("If true, the death effect will start automatically after the animation")]
    public bool autoStartDeathEffect = true;

    private Animator animator;
    private DeathEffect deathEffect;
    private bool isPlayingDeathAnimation = false;

    private void Start()
    {
        // Get required components
        animator = GetComponent<Animator>();
        deathEffect = GetComponent<DeathEffect>();

        // Validate components
        if (animator == null)
        {
            Debug.LogError("PlayerDead: No Animator component found on " + gameObject.name);
        }

        if (deathEffect == null && autoStartDeathEffect)
        {
            Debug.LogWarning("PlayerDead: No DeathEffect component found on " + gameObject.name +
                           ". Death effect will not play automatically.");
        }
    }

   
    public void PlayDeathAnimation()
    {
        if (isPlayingDeathAnimation) return; // Prevent multiple calls

        isPlayingDeathAnimation = true;

    
        if (animator != null)
        {
            // Reset all other animation bools that might interfere
            animator.SetBool("isIdle", false);
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isShooting", false);

            // Reset movement parameters
            animator.SetFloat("horizontal", 0);
            animator.SetFloat("vertical", 0);
            animator.SetFloat("aimX", 0);
            animator.SetFloat("aimY", 0);


            animator.SetBool("isDead", true);

            Debug.Log("Playing death animation: " + deathAnimationName);
        }

        StartCoroutine(HandleDeathSequence());
    }

    private void DisablePlayerControls()
    {
        // Disable movement
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Disable combat
        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }

        // Disable bow
        PlayerBow playerBow = GetComponent<PlayerBow>();
        if (playerBow != null)
        {
            playerBow.enabled = false;
        }

        // Disable form switching
        SwitchForm switchForm = GetComponent<SwitchForm>();
        if (switchForm != null)
        {
            switchForm.enabled = false;
        }

        // Stop player movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; 
            rb.isKinematic = true;
        }


        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }


    private IEnumerator HandleDeathSequence()
    {
    
        yield return new WaitForSeconds(0.1f);


        float animationWaitTime = GetAnimationLength();

        if (animationWaitTime > 0)
        {
            // Wait for actual animation length
            yield return new WaitForSeconds(animationWaitTime);
        }
        else
        {
        
            yield return new WaitForSeconds(deathEffectDelay);
        }


        DisablePlayerControls();

        // Start the death effect
        if (autoStartDeathEffect && deathEffect != null)
        {
            deathEffect.StartDeathEffect();
        }
    }


    private float GetAnimationLength()
    {
        if (animator == null) return 0f;

  
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        if (ac != null)
        {
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == deathAnimationName)
                {
                    return clip.length;
                }
            }
        }

        return 0f; // Couldn't find the animation
    }


    public void OnDeathAnimationComplete()
    {
        Debug.Log("Death animation completed");


        if (!autoStartDeathEffect && deathEffect != null)
        {
            deathEffect.StartDeathEffect();
        }
    }


    public void DisablePlayerControlsDelayed()
    {
        DisablePlayerControls();
    }


    public void TriggerDeathEffect()
    {
        if (deathEffect != null)
        {
            deathEffect.StartDeathEffect();
        }
    }

    public void ResetDeathState()
    {
        isPlayingDeathAnimation = false;

        if (animator != null)
        {
            animator.SetBool("isDead", false);
        }


    }


    public bool IsPlayingDeathAnimation()
    {
        return isPlayingDeathAnimation;
    }


    public bool IsDeathAnimationFinished()
    {
        if (animator == null) return true;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(deathAnimationName) && stateInfo.normalizedTime >= 1.0f;
    }
}
