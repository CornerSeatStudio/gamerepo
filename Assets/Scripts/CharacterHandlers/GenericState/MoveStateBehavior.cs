﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//MoveState represents the current MOVEMENT status of a character
//this is used exclusively by player handlers (which probably is why the constructur's character SHOULD BE a playerhandler, not a characterhandler), too lazy to change
//CombatState and MoveState are switched between in the PlayerHandler

public class IdleMoveState : MoveState {
    public IdleMoveState(CharacterHandler character, Animator animator) : base(character, animator) {}

    public override IEnumerator OnStateEnter() {
        (character as PlayerHandler).ChangeStanceStealthConsequences((character.characterdata as PlayerData).detectionTime, 0f);
        animator.SetBool(Animator.StringToHash("Crouching"), false);

   //     animator.SetBool(Animator.StringToHash("Idle"), true);
        yield break;
    }

    public override IEnumerator OnStateExit() {
       // animator.SetBool(Animator.StringToHash("Idle"), false);
        yield break;    
    }
}

public class JogMoveState : MoveState {
    public JogMoveState(CharacterHandler character, Animator animator) : base(character, animator) {}
    
    public override IEnumerator OnStateEnter() {
        (character as PlayerHandler).ChangeStanceStealthConsequences((character.characterdata as PlayerData).detectionTime /2, 6f);
        animator.SetBool(Animator.StringToHash("Crouching"), false);

       // animator.SetBool(Animator.StringToHash("Jogging"), true);
        (character as PlayerHandler).CurrMovementSpeed = (character.characterdata as PlayerData).jogSpeed;
        yield break;    
    
    }

    public override IEnumerator OnStateExit() {
        //animator.SetBool(Animator.StringToHash("Jogging"), false);
        yield break;    
    }
}

public class SprintMoveState : MoveState {
    IEnumerator StaminaDrain;

    public SprintMoveState(CharacterHandler character, Animator animator) : base(character, animator) {}
    
    public override IEnumerator OnStateEnter() {
        (character as PlayerHandler).ChangeStanceStealthConsequences((character.characterdata as PlayerData).detectionTime /2, 12f);
        animator.SetBool(Animator.StringToHash("Crouching"), false);
       // animator.SetBool(Animator.StringToHash("Sprinting"), true);
        (character as PlayerHandler).CurrMovementSpeed = (character.characterdata as PlayerData).sprintSpeed;
        StaminaDrain = DrainStaminaOverTime();
        character.StartCoroutine(StaminaDrain);
        yield return new WaitUntil(()=>character.Stamina <= 0);
        character.SetStateDriver(new JogMoveState(character, animator));
    }

    private IEnumerator DrainStaminaOverTime() {
        while (character.Stamina > 0) {
            character.DealStamina(.4f);
            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator OnStateExit() {
        if(StaminaDrain != null) character.StopCoroutine(StaminaDrain);
        //animator.SetBool(Animator.StringToHash("Sprinting"), false);
        yield break;    
    }
}

public class WalkMoveState : MoveState {
    public WalkMoveState(CharacterHandler character, Animator animator) : base(character, animator) {}
    
    public override IEnumerator OnStateEnter() {
        (character as PlayerHandler).ChangeStanceStealthConsequences((character.characterdata as PlayerData).detectionTime, 5f);
        animator.SetBool(Animator.StringToHash("Crouching"), false);

       // animator.SetBool(Animator.StringToHash("Walking"), true);
        (character as PlayerHandler).CurrMovementSpeed = (character.characterdata as PlayerData).walkSpeed;
        yield break;    
    }

    public override IEnumerator OnStateExit() {
       // animator.SetBool(Animator.StringToHash("Walking"), false);
        yield break;    
    }
}

public class CrouchIdleMoveState : MoveState {
    public CrouchIdleMoveState(CharacterHandler character, Animator animator) : base(character, animator) {}
    
    public override IEnumerator OnStateEnter() {
        (character as PlayerHandler).ChangeStanceStealthConsequences((character.characterdata as PlayerData).detectionTime * 2.5f, 0f);
        animator.SetBool(Animator.StringToHash("Crouching"), true);
        yield break;    
    }

    public override IEnumerator OnStateExit() {
      //  animator.SetBool(Animator.StringToHash("Crouching"), false);

        yield break;    
    }
}

public class CrouchWalkMoveState : MoveState {
    public CrouchWalkMoveState(CharacterHandler character, Animator animator) : base(character, animator) {}
    
    public override IEnumerator OnStateEnter() {
        (character as PlayerHandler).ChangeStanceStealthConsequences((character.characterdata as PlayerData).detectionTime * 2.3f, 0f);
        animator.SetBool(Animator.StringToHash("Crouching"), true);
        (character as PlayerHandler).CurrMovementSpeed = (character.characterdata as PlayerData).crouchWalkSpeed;        
        yield break;    
    }

    public override IEnumerator OnStateExit() {
        yield break;    
    }
}