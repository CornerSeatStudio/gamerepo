using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefaultState : CombatState {
    public DefaultState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {}

    public override IEnumerator OnStateEnter() {
        //Debug.Log("entering default combat state");
        animator.SetBool(character.AnimationHashes["IsAgro"], true);
        yield break;
    }

    public override IEnumerator OnStateExit() {
        animator.SetBool(character.AnimationHashes["IsAgro"], false);
        //Debug.Log("exiting default combat state");
        yield break;
    }
    //probably just animator stuff
    public override string toString() {
        return "DEFAULT";
    }

}

public class AttackState : CombatState {
    protected IEnumerator currAttackCoroutine;
    public MeleeMove chosenMove {get; private set;}

    public AttackState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {
        chosenMove = character.MeleeAttacks["default"]; 
    }

    public AttackState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler, MeleeMove chosenMove) : base(character, animator, attackHandler) {
        this.chosenMove = chosenMove;
    }
    public override IEnumerator OnStateEnter() { 
        //Debug.Log("entering attacking state");
        currAttackCoroutine = FindTargetAndDealDamage();
        yield return character.StartCoroutine(currAttackCoroutine);
        character.SetStateDriver(new DefaultState(character, animator, attackHandler));
        animator.SetBool(character.AnimationHashes["IsAttacking"], true);
    }    

    protected virtual IEnumerator FindTargetAndDealDamage(){
        yield return character.StartCoroutine(attackHandler.FindTarget(chosenMove));
        
        //Debug.Log(chosenMove.angle + " " + chosenMove.range);
        //if no targets in range
        if (attackHandler.chosenTarget == null) {
            Debug.Log("no targets in range");
            yield return new WaitForSeconds(chosenMove.startup + chosenMove.endlag); //swing anyways
            character.SetStateDriver(new DefaultState(character, animator, attackHandler));
            yield break;
        }
        //upon completion of finding target/at attack move setup, START listening
        yield return new WaitForSeconds(chosenMove.startup); //assumption: start up == counter window


        attackHandler.chosenTarget.AttackResponse(chosenMove.damage, character);
        //during the endlag phase, check again
        //if I was hit && I am using blockable attack, stagger instead
        yield return new WaitForSeconds(chosenMove.endlag);
        character.SetStateDriver(new DefaultState(character, animator, attackHandler));
    }

 
    public override IEnumerator OnStateExit() {
        //Debug.Log("exiting attacking state");
        if(currAttackCoroutine != null) character.StopCoroutine(currAttackCoroutine); 
        animator.SetBool(character.AnimationHashes["IsAttacking"], false);
        yield break;
    }

    public override string toString() {
        return "ATTACK";
    }
}

public class BlockState : CombatState {
    protected IEnumerator currBlockRoutine;
    protected MeleeMove block;

    public BlockState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {
        block = character.MeleeBlock; 
    }

    public override IEnumerator OnStateEnter() { 
        currBlockRoutine = CheckBlockRange();
        animator.SetBool(character.AnimationHashes["IsBlocking"], true);
        yield return character.StartCoroutine(currBlockRoutine);
    } 

    //provides information on blocking CONTINUOUSLY
    private IEnumerator CheckBlockRange() {
        while (true) {
            yield return character.StartCoroutine(attackHandler.FindTarget(block)); //run find target while blocking
            Debug.Log("block would be valid");
            //if the attackHandler.chosenTarget exists, the block IS ALLOWED
        }
    }

    public override IEnumerator OnStateExit() { 
        if(currBlockRoutine != null) character.StopCoroutine(currBlockRoutine); //stop block coroutine
        animator.SetBool(character.AnimationHashes["IsBlocking"], false);
        attackHandler.chosenTarget = null; //empty attackHandler.chosenTarget
        yield break;
    } 

    public override string toString() {
        return "BLOCK";
    }
}

public class CounterState : CombatState {
    protected IEnumerator currCounterRoutine;
    protected MeleeMove block;

    public CounterState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {}

    public override IEnumerator OnStateEnter() {        
        //trigger counter event
        //if enemy is attacking you specifically (dont trigger if coming from a specific state)
        //shouldnt be done here, should be done in attack response via comparing type
        
        animator.SetBool(character.AnimationHashes["IsBlocking"], true);

        currCounterRoutine = CheckCounterRange();
        yield return character.StartCoroutine(currCounterRoutine);


        yield return new WaitForSeconds(0.3f); //where param is counter timeframe
        character.SetStateDriver(new BlockState(character, animator, attackHandler));
    }

    //check ONCE for counter check
    private IEnumerator CheckCounterRange() {
        yield return character.StartCoroutine(attackHandler.FindTarget(block));
    }

    public override IEnumerator OnStateExit() {
        //Debug.Log("exiting counter state");
        if (currCounterRoutine != null) character.StopCoroutine(currCounterRoutine);
        yield break;
    }

    public override string toString() {
        return "COUNTER";
    }
}

public class DodgeState : CombatState {
    public DodgeState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {}

}

public class StaggerState : CombatState {
    public StaggerState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {}

}

public class DeathState : CombatState {
    public DeathState(CharacterHandler character, Animator animator, MeleeRaycastHandler attackHandler) : base(character, animator, attackHandler) {}

    public override IEnumerator OnStateEnter(){
        //change globalState
        (character as AIHandler).GlobalState = AIGlobalState.DEAD;
        //stop APPROPRITE coroutines
        (character as AIHandler).Detection.IsAlive = false; //detection
        //ragdoll 

        Debug.Log("agh i haveth been a slain o");
        yield return null;
    }

    public override string ToString()  {
        return "DEAD";
    }

}