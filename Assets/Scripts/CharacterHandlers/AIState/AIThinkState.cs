﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIThinkState
{
    protected readonly AIHandler character;
    protected Animator animator;
    protected NavMeshAgent agent;

    public AIThinkState(AIHandler character, Animator animator, NavMeshAgent agent) {
        this.character = character;
        this.animator = animator;
        this.agent = agent;
    }

    public virtual IEnumerator OnStateEnter() {
        yield break;
    }

    public virtual IEnumerator OnStateExit() {
        yield break;
    }
    
}
