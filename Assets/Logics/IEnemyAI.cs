using UnityEngine;
public interface IEnemyAI {
    void InitializeAI(); // 初始化行为
    void PerformAction(); // 每帧或定时执行的行为
}