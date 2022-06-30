using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public abstract string GetName();
    public abstract AbilityTargetingType GetTargetingType();
    public abstract Sprite GetIcon();

    protected float cd_timer;
    public AbilitySettings Settings;

    public void Cooldown(float amount) {
        cd_timer -= amount;

        if (cd_timer < 0) { cd_timer = 0; }
    }

    public abstract float GetRange();
    public abstract float GetCooldownLength();
    public abstract float GetCooldown();

    public abstract void Cast(AbilitySettings settings);        // What happens when button is pressed to cast ability
    public abstract void Activate(AbilitySettings settings);    // What happens when the ability finishes casting
    public abstract IEnumerator Effect(AbilitySettings settings);      // What kind of effect the ability has
}

// ==================================    
// Example Ability
// ==================================

public class Example_Ability : Ability
{
    // Private Variables
    private float cooldown = 5.0f;
    private float castRange = 7.0f;

    // Getters / Setters
    public override string GetName() { return "Fishing Rod"; }
    public override AbilityTargetingType GetTargetingType() { return AbilityTargetingType.Directional; }
    public override Sprite GetIcon() { return null; }
    public override float GetRange() { return castRange; }
    public override float GetCooldownLength() { return cooldown; }
    public override float GetCooldown() { return cd_timer; }

    // Function
    public override void Cast(AbilitySettings settings) 
    {
        // Specify a set of conditions that are required before the ability can activate
        // i.e. make sure ability is not on cooldown, that it has a target/direction/position depending on targetingType, etc.
        // if (cd_timer > 0) { Debug.Log("Ability is on cooldown!"); return; } <-- make sure you specify a return after each condition, or else it will still execute.
        
        // Activate the Ability after casting is complete (all conditions have been met)
        Activate(settings);
    }

    public override void Activate(AbilitySettings settings) 
    {
        // make sure you set the cooldown when the ability activates so that we can't just spam the ability
        cd_timer = cooldown;

        // Do whatever you want to do with the ability.

        // For abilities with instant effects, you can just put all the code required in this function. i.e. Deal damage instantly to the target once cast is complete.
        // Instant Effect Example 
        /*
            // I cast a spell that hits the target instantly after I finish casting, regardless of where they are.
            settings.m_Target.DealDamage(10);   // Deal 10 damage to the target when cast is complete.
        */
        
        // For abilities with delayed effects, you'll have to put the effect in the below function called Effect(AbilitySettings settings).
        // --> This is because we don't want the effect to trigger after we cast, but instead when another condition is met.  
        // --> example 1: We shoot a projectile and we only want to trigger the effect after the projectile hits another target.
        // Projectile Activation Example
        /*
            // For this to work, you'll need to create/use a projectile class that uses a delegate to store our effect function and projectile settings
            // Begin by instantiating the projectile prefab object and grabbing the projectile class from it
            GameObject projectleObject = Instantiate(Resources.Load<GameObject>("filepath/for/prefab"), settings.m_Owner.transform.position, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();

            // Next, pass in what we need to make this projectile work the way we want.
            projectile.SetSpeed(projectileSpeed);               // -- We want the projectile to move the speed we specified for this ability
            projectile.OnEffectTrigger += Effect(settings);     // -- We pass in the Effect function to the delegate in the projectile class which gets handled by the projectile class
            projectile.Initialize(...);
        */
        // --> example 2: We trigger a timed or periodic effect which lingers for a period of time.  A buff or a damage over time effect per say.
        // Timed & Period Effect Activation Example
        /*  
            // Because coroutines need to be handled by an object that inherits from MonoBehavior (and our ability class is not)
            // I use a coroutine manager static class to handle these kind of things, which I create as a tool to handle coroutines for me
            // I pass in the same settings to the Effect class so the target and everything else remains the same. 
            CoroutineManager.GetInstance().StartCoroutine(Effect(settings));
        */
        // In these cases, we'll add the code that handles the effect in the below function and either let the Coroutine handle timed and periodic effects,
        // or let the projectile we created call the effect function as a delegate function.  (ask me more about delegate functions) 
    }

    public override IEnumerator Effect(AbilitySettings settings) 
    {   
        // -- Note that none of these will actually work, the variables for them don't exist.  It's an example.

        // Buff Effect Example 
        /*  
            ** Start of Function **

            // Increase player attack power by 10 points
            settings.m_Owner.AttackPower += 10;

            // Wait however long the effect should last. Let's say 1 minute
            yield return new WaitForSeconds(60);

            // Reverse the effect on the player by reducing attack power by 10 points
            settings.m_Owner.AttackPower -= 10;

            **End of Function**
        */

        // Periodic Effect Example
        /*   
            // All together, I will deal 50 damage over a period of 10 seconds.  (5 ticks, 2 second interval, 10 damage).

            // =====================================================================================

            // -- Option A: Looping.  Create a for loop to handle recursive events
            ** Start of Function **

            for (int i = 0; i < 5; i++)     // -- 5 is equal to the number of times I want the ability to happen.  So I'll deal 5 ticks of damage over the duration of my ability. 
            {
                // Wait a period of time to delay the effect.
                yield return new WaitForSeconds(2);     // -- I'll wait 2 seconds between each tick,
            
                // Deal damage to the target
                settings.m_Target.DealDamage(10);       // -- Then I'll deal 10 damage each tick.
            }            
            // And after the for loop ends, the function is over and the effect ends.

            ** End of Function **

            // =====================================================================================

            // -- Option B: Copy Pasta.  Exactly the same thing as above, just structured differently.  
            ** Start of Function ** 
            
            yield return new WaitForSeconds(2);     // Wait 2 seconds   (at 2 seconds)
            settings.m_Target.DealDamage(10);       // Deal 10 damage   (10 damage total)
            yield return new WaitForSeconds(2);     // Wait 2 seconds   (at 4 seconds)
            settings.m_Target.DealDamage(10);       // Deal 10 damage   (20 damage total)
            yield return new WaitForSeconds(2);     // Wait 2 seconds   (at 6 seconds)
            settings.m_Target.DealDamage(10);       // Deal 10 damage   (30 damage total)
            yield return new WaitForSeconds(2);     // Wait 2 seconds   (at 8 seconds)
            settings.m_Target.DealDamage(10);       // Deal 10 damage   (40 damage total)
            yield return new WaitForSeconds(2);     // Wait 2 seconds   (at 10 seconds)
            settings.m_Target.DealDamage(10);       // Deal 10 damage   (50 damage total)            

            ** End of Function ** 
        */


        // Delayed Effect Example (example case for projectile effects)
        /*
            // When the projectile hits a target player, we can then trigger the exact same effect as if it were an instant effect.

            settings.m_Target.DealDamage(10);   // Deal 10 damage to the target when the effect is triggered.
            yield return new WaitForSeconds(0); // Because it's an instant effect no need for a delay.  But because it's a coroutine, we need to specify a yield return statement, so I put 0 seconds.
        */

        yield return new WaitForSeconds(0);
    }
}

// ==================================    
// Abilities
// ==================================

public class Fishing_Rod : Ability
{
    // Private Variables
    private float cooldown = 0.0f;
    private float castRange = 7.0f;

    // Projectile Data
    private float projectileSpeed = 7.0f;

    // Getters / Setters
    public override string GetName() { return "Fishing Rod"; }
    public override AbilityTargetingType GetTargetingType() { return AbilityTargetingType.Directional; }
    public override Sprite GetIcon() { return null; }
    public override float GetRange() { return castRange; }
    public override float GetCooldownLength() { return cooldown; }
    public override float GetCooldown() { return cd_timer; }

    // Function
    public override void Cast(AbilitySettings settings) 
    {
        // Conditions
        if (cd_timer > 0) { Debug.Log("Fishing Rod is on cooldown."); return; }
        if (settings.m_TargetDirection == Vector3.zero) { Debug.Log("Fishing Rod needs a direction to cast."); return; }

        Activate(settings);
    }

    public override void Activate(AbilitySettings settings) 
    {
        cd_timer = cooldown;

        // Create the Projectile
        GameObject projectile = GameObject.Instantiate(Resources.Load<GameObject>("Projectiles/Basic"), settings.m_Owner.transform.position, Quaternion.identity);

        // Set projectile settings        
        ProjectileSettings p_settings = ProjectileSettings.InitializeDirectional(settings.m_Owner, settings.m_TargetDirection, castRange);     
        p_settings.m_ProjectileSpeed = projectileSpeed;
        p_settings.m_AbilitySettings = settings;
        projectile.GetComponent<Projectile>().OnEffectTrigger += Effect;
        projectile.GetComponent<Projectile>().Initialize(p_settings); 

        Debug.Log("Fishing Rod Activated");
    }

    public override IEnumerator Effect(AbilitySettings settings) 
    {
        Debug.Log("Fishing Rod Hit: " + settings.m_Target);
        yield return new WaitForSeconds(0);
    }
}

public class Harpoon : Ability
{
    // Private Variables
    private float cooldown = 0.0f;
    private float castRange = 7.0f;

    // Projectile Data
    private float projectileSpeed = 14.0f;

    // Getters / Setters
    public override string GetName() { return "Harpoon"; }
    public override AbilityTargetingType GetTargetingType() { return AbilityTargetingType.Directional; }
    public override Sprite GetIcon() { return null; }
    public override float GetRange() { return castRange; }
    public override float GetCooldownLength() { return cooldown; }
    public override float GetCooldown() { return cd_timer; }

    // Function
    public override void Cast(AbilitySettings settings) 
    {
        // Conditions
        if (cd_timer > 0) { Debug.Log("Harpoon is on cooldown."); return; }
        if (settings.m_TargetDirection == Vector3.zero) { Debug.Log("Harpoon needs a direction to cast."); return; }

        Activate(settings);
    }

    public override void Activate(AbilitySettings settings) 
    {
        cd_timer = cooldown;

        // Create the Projectile
        GameObject projectile = GameObject.Instantiate(Resources.Load<GameObject>("Projectiles/Basic"), settings.m_Owner.transform.position, Quaternion.identity);

        // Set projectile settings        
        ProjectileSettings p_settings = ProjectileSettings.InitializeDirectional(settings.m_Owner, settings.m_TargetDirection, castRange);     
        p_settings.m_ProjectileSpeed = projectileSpeed;
        p_settings.m_AbilitySettings = settings;
        projectile.GetComponent<Projectile>().OnEffectTrigger += Effect;
        projectile.GetComponent<Projectile>().Initialize(p_settings); 

        Debug.Log("Harpoon Activated");
    }

    public override IEnumerator Effect(AbilitySettings settings) 
    {   
        Debug.Log("Harpoon Hit: " + settings.m_Target);
        yield return new WaitForSeconds(0);
    }
}

public class Air_Blast : Ability
{
    // Private Variables
    private float cooldown = 0.0f;
    private float castRange = 7.0f;

    // Projectile Data
    private float projectileSpeed = 7.0f;

    // Getters / Setters
    public override string GetName() { return "Air Blast"; }
    public override AbilityTargetingType GetTargetingType() { return AbilityTargetingType.Directional; }
    public override Sprite GetIcon() { return null; }
    public override float GetRange() { return castRange; }
    public override float GetCooldownLength() { return cooldown; }
    public override float GetCooldown() { return cd_timer; }

    // Function
    public override void Cast(AbilitySettings settings) 
    {
        // Conditions
        if (cd_timer > 0) { Debug.Log("Air Blast is on cooldown."); return; }
        if (settings.m_TargetDirection == Vector3.zero) { Debug.Log("Air Blast needs a direction to cast."); return; }

        Activate(settings);
    }

    public override void Activate(AbilitySettings settings) 
    {
        cd_timer = cooldown;

        // Create the Projectile
        GameObject projectile = GameObject.Instantiate(Resources.Load<GameObject>("Projectiles/Basic"), settings.m_Owner.transform.position, Quaternion.identity);

        // Set projectile settings        
        ProjectileSettings p_settings = ProjectileSettings.InitializeDirectional(settings.m_Owner, settings.m_TargetDirection, castRange);     
        p_settings.m_ProjectileSpeed = projectileSpeed;
        p_settings.m_AbilitySettings = settings;
        projectile.GetComponent<Projectile>().OnEffectTrigger += Effect;
        projectile.GetComponent<Projectile>().Initialize(p_settings); 

        Debug.Log("Air Blast Activated");
    }


    public override IEnumerator Effect(AbilitySettings settings) 
    {   
        Debug.Log("Air Blast Hit: " + settings.m_Target);
        yield return new WaitForSeconds(0);
    }
}

public class Freeze_Ray : Ability
{
    // Private Variables
    private float cooldown = 0.0f;
    private float castRange = 30.0f;

    // Projectile Data
    private float projectileSpeed = 7.0f;
    private float projectileRadius = 3.0f;

    // Getters / Setters
    public override string GetName() { return "Freeze Ray"; }
    public override AbilityTargetingType GetTargetingType() { return AbilityTargetingType.Positional; }
    public override Sprite GetIcon() { return null; }
    public override float GetRange() { return castRange; }
    public override float GetCooldownLength() { return cooldown; }
    public override float GetCooldown() { return cd_timer; }

    public override void Cast(AbilitySettings settings) 
    {
        // Conditions
        if (cd_timer > 0) { Debug.Log("Freeze Ray is on cooldown."); return; }
        if (settings.m_TargetPosition == Vector3.zero) { Debug.Log("Freeze Ray needs a target position to cast."); return; }
        if (Vector3.Distance(settings.m_TargetPosition, settings.m_Owner.transform.position) > castRange) { Debug.Log("Out of range!"); return; }

        Activate(settings);
    }

    public override void Activate(AbilitySettings settings) 
    {
        cd_timer = cooldown;

        // Create the Projectile
        GameObject projectile = GameObject.Instantiate(Resources.Load<GameObject>("Projectiles/Basic"), settings.m_Owner.transform.position, Quaternion.identity);

        Debug.Log(settings.m_TargetPosition);

        // Set projectile settings            
        ProjectileSettings p_settings = ProjectileSettings.InitializePositional(settings.m_Owner, settings.m_TargetPosition); 
        p_settings.m_ProjectileSpeed = projectileSpeed;
        p_settings.m_ProjectileRadius = projectileRadius;
        p_settings.m_AbilitySettings = settings;
        projectile.GetComponent<Projectile>().OnEffectTrigger += Effect;
        projectile.GetComponent<Projectile>().Initialize(p_settings); 

        Debug.Log("Freeze Ray Activated");
    }

    public override IEnumerator Effect(AbilitySettings settings) 
    {   
        Debug.Log("Freeze Ray Hit: " + settings.m_Target);
        yield return new WaitForSeconds(0);
    }
}

public class Motor_Boost : Ability
{
    // Private Variables
    private float cooldown = 5.0f;
    private float castRange = 7.0f;
    private float duration = 5.0f;

    // Getters / Setters
    public override string GetName() { return "Motor Boost"; }
    public override AbilityTargetingType GetTargetingType() { return AbilityTargetingType.Self; }
    public override Sprite GetIcon() { return null; }
    public override float GetRange() { return castRange; }
    public override float GetCooldownLength() { return cooldown; }
    public override float GetCooldown() { return cd_timer; }

    // Function
    public override void Cast(AbilitySettings settings) 
    {
        // Conditions
        if (cd_timer > 0) { Debug.Log("Motor Boost is on cooldown."); return; }
        if (settings.m_Owner == null) { Debug.Log("Settings for Motor Boost does not have reference to owner."); return; }

        Activate(settings);
    }

    public override void Activate(AbilitySettings settings) 
    {
        cd_timer = cooldown;

        CoroutineManager.Instance.StartCoroutine(Effect(settings));
    }

    public override IEnumerator Effect(AbilitySettings settings) 
    {
        Debug.Log("Motor Ability Activated");

        yield return new WaitForSeconds(duration);

        Debug.Log("Motor Ability Deactivated");
    }
}