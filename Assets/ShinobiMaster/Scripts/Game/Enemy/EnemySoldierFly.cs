using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
    public class EnemySoldierFly : Enemy
    {
        [SerializeField] private List<Transform> points = new List<Transform>();

        [SerializeField] private MoveTrajectory moveTrajectory;
        private bool canMove;



        protected override void Awake()
        {
            base.Awake();
       
            moveTrajectory.Init(points.ToArray(), this);
        }

        protected override void Start()
        {
            base.Start();

            this.canMove = true;
        }

        protected override void Tick(Player target)
        {
            base.Tick(target);
            moveTrajectory.Update();
        }

        public override void StopMove()
        {
            this.canMove = false;
            
            this.moveTrajectory.StopMove();
        }

        public override void StartMove()
        {
            this.canMove = true;
            
            this.moveTrajectory.StartMove();
        }

        [System.Serializable]
        private class MoveTrajectory
        {
            [SerializeField]
            private float speedMove;

            Transform[] points;

            Enemy enemy;

            int index;

            bool ret;
            
            private bool canMove;

            public void Init(Transform[] points, Enemy enemy)
            {
                this.points = points;
                this.enemy = enemy;

                index = 0;
                ret = false;
                enemy.transform.position = points[0].position;

                this.canMove = true;
            }

            public void Update()
            {
                if (!this.canMove)
                {
                    return;
                }
            
                if (index == points.Length - 1)
                {
                    ret = true;
                }
                else if (index == 0)
                {
                    ret = false;
                }

                if (Vector3.Distance(points[index].position, enemy.transform.position) < 0.7f)
                {
                    if (ret)
                    {
                        index--;
                    }
                    else
                    {
                        index++;
                    }
                }

                enemy.transform.position += (points[index].position - enemy.transform.position).normalized * speedMove * Time.deltaTime;
            }
            
            public void StopMove()
            {
                this.canMove = false;
            }

            public void StartMove()
            {
                this.canMove = true;
            }
        }
    }
}