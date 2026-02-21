using UnityEngine;
using UnityEngine; // 必须加上这个，解决 CS0246

public class BossAI : MonoBehaviour, IEnemyAI {
    public void InitializeAI() { 
        Debug.Log("Boss初始化：展示血条并咆哮");
    }

    // 必须实现此方法，解决 CS0535
    public void PerformAction() { 
        // 你可以在这里根据概率决定是 吐毒液 还是 冲刺
        int choice = Random.Range(0, 2);
        if (choice == 0) SplitVenom();
        else Dash();
    }

    public void SplitVenom() { 
        Debug.Log("Boss技能：吐毒液！");
    }

    public void Dash() { 
        Debug.Log("Boss技能：冲刺攻击！");
    }
}