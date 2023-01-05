using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Main : MonoBehaviour {

    public GameObject humanPrefab;
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans;

    void Start() {
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Attack", OnAttack); 
        // NetManager.AddListener("Die", OnDie); // 这里因为还没有定义，所以先 // 掉
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 5000);

        GameObject go = Instantiate(humanPrefab) as GameObject;
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        go.transform.position = new Vector3(x, 0, z);
        myHuman = go.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.Getdesc();
        // 发送协议
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManager.Getdesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y + ",";
        NetManager.Send(sendStr);
        // 请求玩家列表
        NetManager.Send("List|"); // <<<<<<<<<<<<<<<<<<<< 
// 我们还要在客户端Enter协议发送之后,再发送一个List协议的请求,这个请求会让服务器会把目前在线的玩家数据统统广播出去,
// 所以,首先在客户端中发送完Enter协议,再发送List协议,用来获取在线玩家列表,这个List的协议如下:
// 假如目前服务器接收了2位玩家登录.
//     List|127.0.0.1:4565,3,0,5,0,100,127.0.0.1:4556,4,0,9,0,100,
            
        // NetManager.AddListener("Enter", OnEnter);
        // NetManager.AddListener("Move", OnMove);
        // NetManager.AddListener("Leave", OnLeave);
        // NetManager.Connect("127.0.0.1", 5000);
        // GameObject go = Instantiate(humanPrefab) as GameObject;
        // float x = Random.Range(-5, 5);
        // float z = Random.Range(-5, 5);
        // go.transform.position = new Vector3(x, 0, z);
        // myHuman = go.AddComponent<CtrlHuman>();
        // myHuman.desc = NetManager.Getdesc();
        // // 发送协议
        // Vector3 pos = myHuman.transform.position;
        // Vector3 eul = myHuman.transform.eulerAngles;
        // string sendStr = "Enter|";
        // sendStr += NetManager.Getdesc() + ",";
        // sendStr += pos.x + ",";
        // sendStr += pos.y + ",";
        // sendStr += pos.z + ",";
        // sendStr += eul.y;
        // NetManager.Send(sendStr);
    }
// 那么既然发送了List请求,客户端接受到了服务器发来的消息,也应该有OnList的方法供给调用.那么OnList里面应当有所有已连接的客户端信息.
    void OnList(string msgArgs) {
        Debug.Log("OnList" + msgArgs);
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 6;//玩家的数量
        for (int i = 0; i < count; i++) {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            // 如果是自己
            if (desc == NetManager.Getdesc()) {
                continue;
            }
            // 否则添加角色到场景
            GameObject obj = GameObject.Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<syncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }
// 客户端:　通过公式计算,服务器有几台客户端在连接着,解析出它们的参数,并实例化在场景中.
    
    void OnEnter(string msgArgs) {
        Debug.Log("OnEnter" + msgArgs);
        // 解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float euly = float.Parse(split[4]); // 不知道这里是什么意思
        if (desc == NetManager.Getdesc()) {
            return;
        }
        // 增加角色
        GameObject go = Instantiate(humanPrefab)as GameObject;
        go.transform.position = new Vector3(x, y, z);
        go.transform.eulerAngles = new Vector3(0, euly, 0); // 0, 0, 0 ?
        BaseHuman h = go.AddComponent<syncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
    }

    void OnMove(string msgArgs) {
        Debug.Log("OnMove" + msgArgs);
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        h.MoveTo(targetPos);
    }
// bug: 它仍旧会出现,一个客户端出现玩家,另一个客户端并没有出现玩家的现象,这是因为这个客户端没有做消息的正确验证等,所以我们的程序还是有问题的.这个bug后续会解决的.
    
// 客户端:　接收到，有玩家离开/踢出　的来自服务端的广播
    void OnLeave(string msgArgs) {
        Debug.Log("OnLeave" + msgArgs);
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);
        otherHumans.Remove(desc);
    }

    void OnAttack(string msgArgs) {
        Debug.Log("OnAttack" + msgArgs);
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        // 攻击动作
        if (!otherHumans.ContainsKey(desc))
            return;
        syncHuman h = (syncHuman)otherHumans[desc];
        h.SyncAttack(eulY);
    }

    void OnDie(string msgArgs) {
        Debug.Log("OnDie" + msgArgs);
        string[] split = msgArgs.Split(',');
        string attDesc = split[0];
        string hitDesc = split[0];

        if (hitDesc == myHuman.desc) {
            Debug.Log("gameOver");
            return;
        }
        // 自己死了
        if (!otherHumans.ContainsKey(hitDesc)) {
            return;
        }
        syncHuman h = (syncHuman)otherHumans[hitDesc];
        h.gameObject.SetActive(false);
    }
    
    void Update () {
        NetManager.Update();
    }
}