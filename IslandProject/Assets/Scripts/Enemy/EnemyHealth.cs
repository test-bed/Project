﻿using UnityEngine;

namespace CompleteProject
{
    public class EnemyHealth : MonoBehaviour
    {
        public int startingHealth;            // The amount of health the enemy starts the game with.
        public int currentHealth;                   // The current health the enemy has.
        public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip;                 // The sound to play when the enemy dies.


        Animator anim;                              // Reference to the animator.
        AudioSource enemyAudio;                     // Reference to the audio source.
        ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
        CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
        bool isDead;                                // Whether the enemy is dead.
        bool isSinking;                             // Whether the enemy has started sinking through the floor.

        NavMeshAgent nav; 

        void Awake ()
        {




            nav = GetComponent<NavMeshAgent>();

            // Setting up the references.
            anim = GetComponent <Animator> ();
            enemyAudio = GetComponent <AudioSource> ();
            hitParticles = GetComponentInChildren <ParticleSystem> ();
            capsuleCollider = GetComponent <CapsuleCollider> ();


            switch (this.gameObject.name)
            {
                case "ZomBunny(Clone)":
                    startingHealth = 100;
                    
                    break;
                case "ZomBear(Clone)":
                    startingHealth = 200;
                    break;
                case "Hellephant(Clone)":
                    startingHealth = 500;
                    break;
            }

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 5))
            {
                if (hit.collider.gameObject.name == "Water")
                {
                    Destroy(this.gameObject);

                    switch (this.gameObject.name)
                    {
                        case "ZomBunny(Clone)":
                            EnemyManager.Obj_Cnt[0]--;
                            break;
                        case "ZomBear(Clone)":
                            EnemyManager.Obj_Cnt[1]--;
                            break;
                        case "Hellephant(Clone)":
                            EnemyManager.Obj_Cnt[2]--;
                            break;
                    }

                }
            }
        }


        void Update ()
        {
            // If the enemy should be sinking...
            if(isSinking)
            {
                // ... move the enemy down by the sinkSpeed per second.
                transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
            }
        }


        public void TakeDamage (int amount, Vector3 hitPoint)
        {
            // If the enemy is dead...
            if(isDead)
                // ... no need to take damage so exit the function.
                return;

            // Play the hurt sound effect.
            enemyAudio.Play ();

            // Reduce the current health by the amount of damage sustained.
            currentHealth -= amount;
            
            // Set the position of the particle system to where the hit was sustained.
            hitParticles.transform.position = hitPoint;

            // And play the particles.
            hitParticles.Play();

            // If the current health is less than or equal to zero...
            if(currentHealth <= 0)
            {
                // ... the enemy is dead.
                Death ();
            }
        }


        void Death ()
        {
            // The enemy is dead.
            isDead = true;

            // Turn the collider into a trigger so shots can pass through it.
            capsuleCollider.isTrigger = true;
            switch (this.gameObject.name)
            {
                case "ZomBunny(Clone)":
                    EnemyManager.Obj_Cnt[0]--;
                    break;
                case "ZomBear(Clone)":
                    EnemyManager.Obj_Cnt[1]--;
                    break;
                case "Hellephant(Clone)":
                    EnemyManager.Obj_Cnt[2]--;
                    break;
            }
            // Tell the animator that the enemy is dead.
            anim.SetTrigger ("Dead");

            // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
            enemyAudio.clip = deathClip;
            enemyAudio.Play ();
        }


        public void StartSinking ()
        {
            // Find and disable the Nav Mesh Agent.
            GetComponent <NavMeshAgent> ().enabled = false;

            // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
            GetComponent <Rigidbody> ().isKinematic = true;

            // The enemy should no sink.
            isSinking = true;

            // Increase the score by the enemy's score value.
            ScoreManager.score += scoreValue;

            // After 2 seconds destory the enemy.
            Destroy (gameObject, 2f);
        }


    }
}