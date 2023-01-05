using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {

    public class BaseHuman : MonoBehaviour {

        protected bool isMoving = false;

        private Vector3 TargetPosition;
        public float speed = 1.2f;
        private Animator animator;
        public string desc = "";

        public void MoveTo(Vector3 pos) {
            TargetPosition = pos;
            isMoving = true;
            animator.SetBool("isMoving", true);
        }
// 这里不知道是动画的原因,还是哪些细节没弄好,人物没有真正移动,只有由原地站立变成了原地跑动,它应用跑走才对,与模型相关吗? 
        public void MoveUpdate() {
            if (isMoving == false)
                return;
            Vector3 pos = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
            transform.LookAt(TargetPosition);
            if (Vector3.Distance(transform.position, TargetPosition) < 0.05f) {
                isMoving = false;
                animator.SetBool("isMoving", false );
            }
        }
        public void Start () {
            animator = transform.GetComponent<Animator>();
        }

        internal bool isAttacking = false;
        internal float attackTime = float.MinValue;

        public void Attack() {
            isAttacking = true;
            attackTime = Time.time;
            animator.SetBool("isAttacking", true);
        }

        public void AttackUpdate() {
            if (!isAttacking) return;
            if (Time.time - attackTime < 1.2f) return;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }

        public void Update() {
            MoveUpdate();
            AttackUpdate();
        }
    }
}