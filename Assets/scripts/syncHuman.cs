using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class syncHuman : BaseHuman {
        new void Start () {
            base.Start();
        }

        new void Update () {
            base.Update();
        }

        public void SyncAttack(float eulY) {
            transform.eulerAngles = new Vector3(0, eulY, 0);
            Attack();
        }
	}
}