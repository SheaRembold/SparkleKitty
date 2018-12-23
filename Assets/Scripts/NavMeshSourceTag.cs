using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    public bool useColliders;

    // Global containers for all active mesh/terrain tags
    public static List<MeshFilter> m_Meshes = new List<MeshFilter>();
    public static List<Terrain> m_Terrains = new List<Terrain>();
    public static List<BoxCollider> m_Colliders = new List<BoxCollider>();

    void OnEnable()
    {
        if (useColliders)
        {
            var c = GetComponentsInChildren<BoxCollider>();
            if (c != null)
            {
                m_Colliders.AddRange(c);
            }
        }
        else
        {
            var m = GetComponentsInChildren<MeshFilter>();
            if (m != null)
            {
                m_Meshes.AddRange(m);
            }

            var t = GetComponentsInChildren<Terrain>();
            if (t != null)
            {
                m_Terrains.AddRange(t);
            }
        }
    }

    void OnDisable()
    {
        var m = GetComponentsInChildren<MeshFilter>();
        if (m != null)
        {
            for (int i = 0; i < m.Length; i++)
                m_Meshes.Remove(m[i]);
        }

        var t = GetComponentsInChildren<Terrain>();
        if (t != null)
        {
            for (int i = 0; i < t.Length; i++)
                m_Terrains.Remove(t[i]);
        }

        var c = GetComponentsInChildren<BoxCollider>();
        if (c != null)
        {
            for (int i = 0; i < c.Length; i++)
                m_Colliders.Remove(c[i]);
        }
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (var i = 0; i < m_Meshes.Count; ++i)
        {
            var mf = m_Meshes[i];
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = 0;
            sources.Add(s);
        }

        for (var i = 0; i < m_Terrains.Count; ++i)
        {
            var t = m_Terrains[i];
            if (t == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Terrain;
            s.sourceObject = t.terrainData;
            // Terrain system only supports translation - so we pass translation only to back-end
            s.transform = Matrix4x4.TRS(t.transform.position, Quaternion.identity, Vector3.one);
            s.area = 0;
            sources.Add(s);
        }

        for (var i = 0; i < m_Colliders.Count; ++i)
        {
            var c = m_Colliders[i];
            if (c == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Box;
            s.size = c.size;
            s.transform = Matrix4x4.TRS(c.transform.TransformPoint(c.center), c.transform.rotation, c.transform.lossyScale);
            s.area = 0;
            sources.Add(s);
        }
    }
}
