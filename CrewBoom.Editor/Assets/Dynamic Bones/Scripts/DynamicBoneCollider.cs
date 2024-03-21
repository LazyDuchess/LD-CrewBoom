using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Collider")]
public class DynamicBoneCollider : DynamicBoneColliderBase
{
#if UNITY_5_3_OR_NEWER
    [Tooltip("The radius of the sphere or capsule.")]
#endif	
    public float m_Radius = 0.5f;

#if UNITY_5_3_OR_NEWER
    [Tooltip("The height of the capsule.")]
#endif		
    public float m_Height = 0;

#if UNITY_5_3_OR_NEWER
    [Tooltip("The other radius of the capsule.")]
#endif	
    public float m_Radius2 = 0;

    // prepare data
    float m_ScaledRadius;
    float m_ScaledRadius2;
    Vector3 m_C0;
    Vector3 m_C1;
    float m_C01Distance;
    int m_CollideType;

    void OnValidate()
    {
        m_Radius = Mathf.Max(m_Radius, 0);
        m_Height = Mathf.Max(m_Height, 0);
        m_Radius2 = Mathf.Max(m_Radius2, 0);
    }

    public override void Prepare()
    {
        float scale = Mathf.Abs(transform.lossyScale.x);
        float halfHeight = m_Height * 0.5f;

        if (m_Radius2 <= 0 || Mathf.Abs(m_Radius - m_Radius2) < 0.01f)
        {
            m_ScaledRadius = m_Radius * scale;

            float h = halfHeight - m_Radius;
            if (h <= 0)
            {
                m_C0 = transform.TransformPoint(m_Center);

                if (m_Bound == Bound.Outside)
                {
                    m_CollideType = 0;
                }
                else
                {
                    m_CollideType = 1;
                }
            }
            else
            {
                Vector3 c0 = m_Center;
                Vector3 c1 = m_Center;

                switch (m_Direction)
                {
                    case Direction.X:
                        c0.x += h;
                        c1.x -= h;
                        break;
                    case Direction.Y:
                        c0.y += h;
                        c1.y -= h;
                        break;
                    case Direction.Z:
                        c0.z += h;
                        c1.z -= h;
                        break;
                }

                m_C0 = transform.TransformPoint(c0);
                m_C1 = transform.TransformPoint(c1);
                m_C01Distance = (m_C1 - m_C0).magnitude;

                if (m_Bound == Bound.Outside)
                {
                    m_CollideType = 2;
                }
                else
                {
                    m_CollideType = 3;
                }
            }
        }
        else
        {
            float r = Mathf.Max(m_Radius, m_Radius2);
            if (halfHeight - r <= 0)
            {
                m_ScaledRadius = r * scale;
                m_C0 = transform.TransformPoint(m_Center);

                if (m_Bound == Bound.Outside)
                {
                    m_CollideType = 0;
                }
                else
                {
                    m_CollideType = 1;
                }
            }
            else
            {
                m_ScaledRadius = m_Radius * scale;
                m_ScaledRadius2 = m_Radius2 * scale;

                float h0 = halfHeight - m_Radius;
                float h1 = halfHeight - m_Radius2;
                Vector3 c0 = m_Center;
                Vector3 c1 = m_Center;

                switch (m_Direction)
                {
                    case Direction.X:
                        c0.x += h0;
                        c1.x -= h1;
                        break;
                    case Direction.Y:
                        c0.y += h0;
                        c1.y -= h1;
                        break;
                    case Direction.Z:
                        c0.z += h0;
                        c1.z -= h1;
                        break;
                }

                m_C0 = transform.TransformPoint(c0);
                m_C1 = transform.TransformPoint(c1);
                m_C01Distance = (m_C1 - m_C0).magnitude;

                if (m_Bound == Bound.Outside)
                {
                    m_CollideType = 4;
                }
                else
                {
                    m_CollideType = 5;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!enabled)
            return;

        Prepare();

        if (m_Bound == Bound.Outside)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.magenta;
        }

        switch (m_CollideType)
        {
            case 0:
            case 1:
                Gizmos.DrawWireSphere(m_C0, m_ScaledRadius);
                break;
            case 2:
            case 3:
                DrawCapsule(m_C0, m_C1, m_ScaledRadius, m_ScaledRadius);
                break;
            case 4:
            case 5:
                DrawCapsule(m_C0, m_C1, m_ScaledRadius, m_ScaledRadius2);
                break;
        }
    }

    static void DrawCapsule(Vector3 c0, Vector3 c1, float radius0, float radius1)
    {
        Gizmos.DrawLine(c0, c1);
        Gizmos.DrawWireSphere(c0, radius0);
        Gizmos.DrawWireSphere(c1, radius1);
    }
}
