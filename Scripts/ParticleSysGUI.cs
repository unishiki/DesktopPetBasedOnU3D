using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSysGUI : MonoBehaviour
{
    public List<Vector3> nodes;
    public Vector3[] directions;
    private ParticleSystem particles;
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        particles.startLifetime = nodes.Count;
        if (nodes.Count == 0)
            Debug.LogError("请添加至少1个node");
        //自动生成方向
        directions = new Vector3[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
            directions[i] = (nodes[i] - ((i - 1 >= 0) ? nodes[i - 1] : transform.position));
    }
    void LateUpdate()
    {
        ParticleSystem.Particle[] particleList = new ParticleSystem.Particle[particles.particleCount];
        int partCount = particles.GetParticles(particleList);
        for (int i = 0; i < partCount; i++)
        {
            // 计算粒子当前的生命
            float timeALive = particleList[i].startLifetime - particleList[i].remainingLifetime;
            float dist = GetAddedMagnitude((int)timeALive);
            int count = 0;
            //判断位置信息
            while (dist > GetAddedMagnitude(count))
            {
                count++;
                particleList[i].velocity = directions[count];
            }
        }
        particles.SetParticles(particleList, partCount);
    }
    private float GetAddedMagnitude(int count)
    {
        float addedMagnitude = 0;
        for (int i = 0; i < count; i++)
        {
            addedMagnitude += directions[i].magnitude;
        }
        return addedMagnitude;
    }

}