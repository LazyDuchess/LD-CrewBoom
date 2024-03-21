#if UNITY_WEBGL
// No multithread
#else
#define ENABLE_MULTITHREAD
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Threading;

[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
#if UNITY_5_3_OR_NEWER
    [Tooltip("The roots of the transform hierarchy to apply physics.")]
#endif
    public Transform m_Root = null;
    public List<Transform> m_Roots = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("Internal physics simulation rate.")]
#endif
    public float m_UpdateRate = 60.0f;

    public enum UpdateMode
    {
        Normal,
        AnimatePhysics,
        UnscaledTime,
        Default
    }
    public UpdateMode m_UpdateMode = UpdateMode.Default;

#if UNITY_5_3_OR_NEWER
    [Tooltip("How much the bones slowed down.")]
#endif
    [Range(0, 1)]
    public float m_Damping = 0.1f;
    public AnimationCurve m_DampingDistrib = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("How much the force applied to return each bone to original orientation.")]
#endif
    [Range(0, 1)]
    public float m_Elasticity = 0.1f;
    public AnimationCurve m_ElasticityDistrib = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("How much bone's original orientation are preserved.")]
#endif
    [Range(0, 1)]
    public float m_Stiffness = 0.1f;
    public AnimationCurve m_StiffnessDistrib = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("How much character's position change is ignored in physics simulation.")]
#endif
    [Range(0, 1)]
    public float m_Inert = 0;
    public AnimationCurve m_InertDistrib = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("How much the bones slowed down when collide.")]
#endif
    public float m_Friction = 0;
    public AnimationCurve m_FrictionDistrib = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("Each bone can be a sphere to collide with colliders. Radius describe sphere's size.")]
#endif
    public float m_Radius = 0;
    public AnimationCurve m_RadiusDistrib = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("If End Length is not zero, an extra bone is generated at the end of transform hierarchy.")]
#endif
    public float m_EndLength = 0;

#if UNITY_5_3_OR_NEWER
    [Tooltip("If End Offset is not zero, an extra bone is generated at the end of transform hierarchy.")]
#endif
    public Vector3 m_EndOffset = Vector3.zero;

#if UNITY_5_3_OR_NEWER
    [Tooltip("The force apply to bones. Partial force apply to character's initial pose is cancelled out.")]
#endif
    public Vector3 m_Gravity = Vector3.zero;

#if UNITY_5_3_OR_NEWER
    [Tooltip("The force apply to bones.")]
#endif
    public Vector3 m_Force = Vector3.zero;

#if UNITY_5_3_OR_NEWER
    [Tooltip("Control how physics blends with existing animation.")]
#endif
    [Range(0, 1)]
    public float m_BlendWeight = 1.0f;

#if UNITY_5_3_OR_NEWER
    [Tooltip("Collider objects interact with the bones.")]
#endif
    public List<DynamicBoneColliderBase> m_Colliders = null;

#if UNITY_5_3_OR_NEWER
    [Tooltip("Bones exclude from physics simulation.")]
#endif
    public List<Transform> m_Exclusions = null;

    public enum FreezeAxis
    {
        None, X, Y, Z
    }
#if UNITY_5_3_OR_NEWER
    [Tooltip("Constrain bones to move on specified plane.")]
#endif	
    public FreezeAxis m_FreezeAxis = FreezeAxis.None;

#if UNITY_5_3_OR_NEWER
    [Tooltip("Disable physics simulation automatically if character is far from camera or player.")]
#endif
    public bool m_DistantDisable = false;
    public Transform m_ReferenceObject = null;
    public float m_DistanceToObject = 20;

    [HideInInspector]
    public bool m_Multithread = true;

    Vector3 m_ObjectMove;
    Vector3 m_ObjectPrevPosition;
    float m_ObjectScale;

    float m_Time = 0;
    float m_Weight = 1.0f;
    bool m_DistantDisabled = false;
    int m_PreUpdateCount = 0;

    class Particle
    {
        public Transform m_Transform;
        public int m_ParentIndex;
        public int m_ChildCount;
        public float m_Damping;
        public float m_Elasticity;
        public float m_Stiffness;
        public float m_Inert;
        public float m_Friction;
        public float m_Radius;
        public float m_BoneLength;
        public bool m_isCollide;
        public bool m_TransformNotNull;

        public Vector3 m_Position;
        public Vector3 m_PrevPosition;
        public Vector3 m_EndOffset;
        public Vector3 m_InitLocalPosition;
        public Quaternion m_InitLocalRotation;

        // prepare data
        public Vector3 m_TransformPosition;
        public Vector3 m_TransformLocalPosition;
        public Matrix4x4 m_TransformLocalToWorldMatrix;
    }

    class ParticleTree
    {
        public Transform m_Root;
        public Vector3 m_LocalGravity;
        public Matrix4x4 m_RootWorldToLocalMatrix;
        public float m_BoneTotalLength;
        public List<Particle> m_Particles = new List<Particle>();

        // prepare data
        public Vector3 m_RestGravity;
    }

    List<ParticleTree> m_ParticleTrees = new List<ParticleTree>();

    // prepare data
    float m_DeltaTime;
    List<DynamicBoneColliderBase> m_EffectiveColliders;

#if ENABLE_MULTITHREAD
    // multithread
    bool m_WorkAdded = false;
    static List<DynamicBone> s_PendingWorks = new List<DynamicBone>();
    static List<DynamicBone> s_EffectiveWorks = new List<DynamicBone>();
    static AutoResetEvent s_AllWorksDoneEvent;
    static int s_RemainWorkCount;
    static Semaphore s_WorkQueueSemaphore;
    static int s_WorkQueueIndex;
#endif

    static int s_UpdateCount;
    static int s_PrepareFrame;
 
    void OnValidate()
    {
        m_UpdateRate = Mathf.Max(m_UpdateRate, 0);
        m_Damping = Mathf.Clamp01(m_Damping);
        m_Elasticity = Mathf.Clamp01(m_Elasticity);
        m_Stiffness = Mathf.Clamp01(m_Stiffness);
        m_Inert = Mathf.Clamp01(m_Inert);
        m_Friction = Mathf.Clamp01(m_Friction);
        m_Radius = Mathf.Max(m_Radius, 0);
    }

    void OnDrawGizmosSelected()
    {
        if (!enabled)
            return;

        if (Application.isEditor && !Application.isPlaying && transform.hasChanged)
        {
            //InitTransforms();
            SetupParticles();
        }

        Gizmos.color = Color.white;
        for (int i = 0; i < m_ParticleTrees.Count; ++i)
        {
            DrawGizmos(m_ParticleTrees[i]);
        }
    }

    void DrawGizmos(ParticleTree pt)
    {
        for (int i = 0; i < pt.m_Particles.Count; ++i)
        {
            Particle p = pt.m_Particles[i];
            if (p.m_ParentIndex >= 0)
            {
                Particle p0 = pt.m_Particles[p.m_ParentIndex];
                Gizmos.DrawLine(p.m_Position, p0.m_Position);
            }

            if (p.m_Radius > 0)
            {
                Gizmos.DrawWireSphere(p.m_Position, p.m_Radius * m_ObjectScale);
            }
        }
    }

    public void SetWeight(float w)
    {
        if (m_Weight != w)
        {
            if (w == 0)
            {
                InitTransforms();
            }
            else if (m_Weight == 0)
            {
                ResetParticlesPosition();
            }
            m_Weight = m_BlendWeight = w;
        }
    }

    public void SetupParticles()
    {
        m_ParticleTrees.Clear();

        if (m_Root != null)
        {
            AppendParticleTree(m_Root);
        }

        if (m_Roots != null)
        {
            for (int i = 0; i < m_Roots.Count; ++i)
            {
                Transform root = m_Roots[i];
                if (root == null)
                    continue;

                if (m_ParticleTrees.Exists(x => x.m_Root == root))
                    continue;

                AppendParticleTree(root);
            }
        }

        m_ObjectScale = Mathf.Abs(transform.lossyScale.x);
        m_ObjectPrevPosition = transform.position;
        m_ObjectMove = Vector3.zero;

        for (int i = 0; i < m_ParticleTrees.Count; ++i)
        {
            ParticleTree pt = m_ParticleTrees[i];
            AppendParticles(pt, pt.m_Root, -1, 0);
        }

        UpdateParameters();
    }

    void AppendParticleTree(Transform root)
    {
        if (root == null)
            return;

        var pt = new ParticleTree();
        pt.m_Root = root;
        pt.m_RootWorldToLocalMatrix = root.worldToLocalMatrix;
        m_ParticleTrees.Add(pt);
    }

    void AppendParticles(ParticleTree pt, Transform b, int parentIndex, float boneLength)
    {
        var p = new Particle();
        p.m_Transform = b;
        p.m_TransformNotNull = b != null;
        p.m_ParentIndex = parentIndex;

        if (b != null)
        {
            p.m_Position = p.m_PrevPosition = b.position;
            p.m_InitLocalPosition = b.localPosition;
            p.m_InitLocalRotation = b.localRotation;
        }
        else 	// end bone
        {
            Transform pb = pt.m_Particles[parentIndex].m_Transform;
            if (m_EndLength > 0)
            {
                Transform ppb = pb.parent;
                if (ppb != null)
                {
                    p.m_EndOffset = pb.InverseTransformPoint((pb.position * 2 - ppb.position)) * m_EndLength;
                }
                else
                {
                    p.m_EndOffset = new Vector3(m_EndLength, 0, 0);
                }
            }
            else
            {
                p.m_EndOffset = pb.InverseTransformPoint(transform.TransformDirection(m_EndOffset) + pb.position);
            }
            p.m_Position = p.m_PrevPosition = pb.TransformPoint(p.m_EndOffset);
            p.m_InitLocalPosition = Vector3.zero;
            p.m_InitLocalRotation = Quaternion.identity;
        }

        if (parentIndex >= 0)
        {
            boneLength += (pt.m_Particles[parentIndex].m_Transform.position - p.m_Position).magnitude;
            p.m_BoneLength = boneLength;
            pt.m_BoneTotalLength = Mathf.Max(pt.m_BoneTotalLength, boneLength);
            ++pt.m_Particles[parentIndex].m_ChildCount;
        }

        int index = pt.m_Particles.Count;
        pt.m_Particles.Add(p);

        if (b != null)
        {
            for (int i = 0; i < b.childCount; ++i)
            {
                Transform child = b.GetChild(i);
                bool exclude = false;
                if (m_Exclusions != null)
                {
                    exclude = m_Exclusions.Contains(child);
                }
                if (!exclude)
                {
                    AppendParticles(pt, child, index, boneLength);
                }
                else if (m_EndLength > 0 || m_EndOffset != Vector3.zero)
                {
                    AppendParticles(pt, null, index, boneLength);
                }
            }

            if (b.childCount == 0 && (m_EndLength > 0 || m_EndOffset != Vector3.zero))
            {
                AppendParticles(pt, null, index, boneLength);
            }
        }
    }

    public void UpdateParameters()
    {
        SetWeight(m_BlendWeight);

        for (int i = 0; i < m_ParticleTrees.Count; ++i)
        {
            UpdateParameters(m_ParticleTrees[i]);
        }
    }

    void UpdateParameters(ParticleTree pt)
    {
        // m_LocalGravity = m_Root.InverseTransformDirection(m_Gravity);
        pt.m_LocalGravity = pt.m_RootWorldToLocalMatrix.MultiplyVector(m_Gravity).normalized * m_Gravity.magnitude;

        for (int i = 0; i < pt.m_Particles.Count; ++i)
        {
            Particle p = pt.m_Particles[i];
            p.m_Damping = m_Damping;
            p.m_Elasticity = m_Elasticity;
            p.m_Stiffness = m_Stiffness;
            p.m_Inert = m_Inert;
            p.m_Friction = m_Friction;
            p.m_Radius = m_Radius;

            if (pt.m_BoneTotalLength > 0)
            {
                float a = p.m_BoneLength / pt.m_BoneTotalLength;
                if (m_DampingDistrib != null && m_DampingDistrib.keys.Length > 0)
                    p.m_Damping *= m_DampingDistrib.Evaluate(a);
                if (m_ElasticityDistrib != null && m_ElasticityDistrib.keys.Length > 0)
                    p.m_Elasticity *= m_ElasticityDistrib.Evaluate(a);
                if (m_StiffnessDistrib != null && m_StiffnessDistrib.keys.Length > 0)
                    p.m_Stiffness *= m_StiffnessDistrib.Evaluate(a);
                if (m_InertDistrib != null && m_InertDistrib.keys.Length > 0)
                    p.m_Inert *= m_InertDistrib.Evaluate(a);
                if (m_FrictionDistrib != null && m_FrictionDistrib.keys.Length > 0)
                    p.m_Friction *= m_FrictionDistrib.Evaluate(a);
                if (m_RadiusDistrib != null && m_RadiusDistrib.keys.Length > 0)
                    p.m_Radius *= m_RadiusDistrib.Evaluate(a);
            }

            p.m_Damping = Mathf.Clamp01(p.m_Damping);
            p.m_Elasticity = Mathf.Clamp01(p.m_Elasticity);
            p.m_Stiffness = Mathf.Clamp01(p.m_Stiffness);
            p.m_Inert = Mathf.Clamp01(p.m_Inert);
            p.m_Friction = Mathf.Clamp01(p.m_Friction);
            p.m_Radius = Mathf.Max(p.m_Radius, 0);
        }
    }

    void InitTransforms()
    {
        for (int i = 0; i < m_ParticleTrees.Count; ++i)
        {
            InitTransforms(m_ParticleTrees[i]);
        }
    }

    void InitTransforms(ParticleTree pt)
    {
        for (int i = 0; i < pt.m_Particles.Count; ++i)
        {
            Particle p = pt.m_Particles[i];
            if (p.m_TransformNotNull)
            {
                p.m_Transform.localPosition = p.m_InitLocalPosition;
                p.m_Transform.localRotation = p.m_InitLocalRotation;
            }
        }
    }

    void ResetParticlesPosition()
    {
        for (int i = 0; i < m_ParticleTrees.Count; ++i)
        {
            ResetParticlesPosition(m_ParticleTrees[i]);
        }

        m_ObjectPrevPosition = transform.position;
    }

    void ResetParticlesPosition(ParticleTree pt)
    {
        for (int i = 0; i < pt.m_Particles.Count; ++i)
        {
            Particle p = pt.m_Particles[i];
            if (p.m_TransformNotNull)
            {
                p.m_Position = p.m_PrevPosition = p.m_Transform.position;
            }
            else	// end bone
            {
                Transform pb = pt.m_Particles[p.m_ParentIndex].m_Transform;
                p.m_Position = p.m_PrevPosition = pb.TransformPoint(p.m_EndOffset);
            }
            p.m_isCollide = false;
        }
    }
}
