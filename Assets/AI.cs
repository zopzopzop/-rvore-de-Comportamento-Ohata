using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

namespace Panda.Examples.Shooter
{

    public class AI : MonoBehaviour
    {
        public Transform player;
        public Transform bulletSpawn;
        public Slider healthBar;
        public GameObject bulletPrefab;

        NavMeshAgent agent;
        public Vector3 destination;
        public Vector3 Target;
        float health = 100.0f;
        float rootSpeed = 5.0f;

        float visiblerange = 80.0f;
        float shotRange = 40.0f;


            Unit enemy;
        Unit self;
        AIVision vision;

        float random_destination_radius = 1.0f;

        Vector3 enemyLastSeenPosition;

       

       

        [Task]
        public void PickRandomDestination()
        {
            //seta a posição onde meu inimigo vai se movimentar
            Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            agent.SetDestination(dest);
            Task.current.Succeed();
        }

        [Task]
        public void MoveToDestination()
        //movimenta meu inimigo para uma posição do PickRandomDestination
        {
            if (Task.isInspected)
                Task.current.debugInfo = string.Format("t = {0:0.00}", Time.time);
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                Task.current.Succeed();
            }
        }

        [Task] 
        public void TargetPlayer() 
        { //encontra meu player
            Target = player.transform.position; 
            Task.current.Succeed(); 
        }
        [Task] 
        public bool Fire() 
        { //Faz o bot atirar
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation); 
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
            return true; 
        }
        [Task] 
        public void LookAtTarget() 
        {
            //Faz meu Bot olha para o player
            Vector3 direction = Target - this.transform.position; this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rootSpeed); 
            if (Task.isInspected) 
                Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction)); 
            if (Vector3.Angle(this.transform.forward, direction) < 5.0f) 
            {
                Task.current.Succeed(); 
            } 
        }

        [Task]
        public bool SeePlayer()
        {
            //identifica Meu Player

            Vector3 distance = player.transform.position - this.transform.position;
            RaycastHit hit;
            bool seeWall = false;
            Debug.DrawRay(this.transform.position, distance, Color.red);
            if (Physics.Raycast(this.transform.position, distance, out hit))
            {
                //identifica as paredes do cenário
                if (hit.collider.gameObject.tag == "wall")
                {
                    seeWall = true;
                }
            }
            if (Task.isInspected)
                Task.current.debugInfo = string.Format("wall={0}", seeWall);

            if (distance.magnitude < visiblerange && !seeWall) //retorna se encontrou meu player
            {
                return true;
            }
            else
            {
                return false;//caso não retorna falço
            }
        }

        [Task] 
        bool Turn(float angle) 

        { //rotaciona o movimento suave do meu player
            var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward; 
            Target = p; 
            return true; 
        }

        // Use this for initialization
        void Start()
        {
            agent = this.GetComponent<NavMeshAgent>();
            agent.stoppingDistance = shotRange - 5;
            InvokeRepeating("UpdateHealth", 5, 0.5f);

            
        }
        private void Update()
        {
            Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position); 
            healthBar.value = (int)health; 
            healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
        }
       void UpdateHealth() 
        {
            if (health < 100) 
                health++;

        }

        //Verifica o HP do bot

        [Task]
        public bool IsHealthLessThan(float health)
        {
            return this.health < health;
        }

        //Destrio meu bot
        [Task]
        public bool Explode()
        {
            Destroy(healthBar.gameObject);
            Destroy(this.gameObject);
            return true;
        }


        //Tira HP Do Inimigo
        public void Damege()
        {
            health -= 25;
            if (health < 10)
            {
                Explode();
            }
        }


    }
}
