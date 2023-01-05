using UnityEngine;

namespace Assets.Scripts {

    public class CtrlHuman : BaseHuman {
        private const string TAG = "CtrlHuman";

       new void Start() {
            base.Start();
        }
       public new void Update() {
            base.Update();
            Debug.Log(TAG + " (Input.GetMouseButtonDown(0)): " + (Input.GetMouseButtonDown(0)));
            
            if (Input.GetMouseButtonDown(0)) { // 鼠标左键
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider.tag.Equals("Terrain")) {
                    MoveTo(hit.point);
                    // 终点消息发送到服务器
                    string sendStr = "Move|";
                    sendStr += NetManager.Getdesc() + ",";
                    sendStr += hit.point.x + ",";
                    sendStr += hit.point.y + ",";
                    sendStr += hit.point.z + ",";
                    NetManager.Send(sendStr);                    
                }
            } else if (Input.GetMouseButtonDown(1)) { // 鼠标右键: 攻击 在做攻击动作的时候做一个攻击判定:
                if (isAttacking) return;
                if (isMoving) return;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray,out hit);
                transform.LookAt(hit.point);
                Attack();
                string sendStr = "Attack|";
                sendStr += NetManager.Getdesc()+",";
                sendStr += transform.eulerAngles.y + ",";
                NetManager.Send(sendStr);

                //  击判定
                Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
                Vector3 lineStart = lineEnd + 20 * transform.forward;
                if (Physics.Linecast(lineStart, lineEnd, out hit)) {
					Debug.DrawLine(lineStart, lineEnd);
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj == gameObject)
                        return;
                    syncHuman h = (syncHuman)hitObj.GetComponent<syncHuman>();
                    if (h == null)
                        return;
                    sendStr = "Hit|";
                    sendStr += NetManager.Getdesc() + ",";
                    sendStr += h.desc + ",";
                    NetManager.Send(sendStr);
                    Debug.Log(sendStr);
                }
            }
        }
    }
}
 