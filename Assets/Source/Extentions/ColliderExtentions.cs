using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ColliderExtentions
{
    public static IEnumerable<Collider> Overlap(this BoxCollider boxCollider,
                                     int layerMask = int.MaxValue,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                                     bool enableDebugMode = false, Vector3 offsetPosition = new Vector3())
    {
        Transform boxTransform = boxCollider.transform;

        Vector3 center = boxTransform.TransformPoint(boxCollider.center) + offsetPosition;
        Vector3 halfSize = Vector3.Scale(boxCollider.size, boxTransform.lossyScale) / 2;

        if (enableDebugMode)
            Debug.DrawCube(center, boxTransform.rotation, halfSize, Color.yellow);

        return Physics.OverlapBox(center, halfSize, boxTransform.rotation, layerMask, queryTriggerInteraction).Where(x => x != boxCollider);
    }

    public static IEnumerable<RaycastHit> CastAll(this BoxCollider boxCollider,
                                     Vector3 direction,
                                     float maxDistance,
                                     int layerMask = int.MaxValue,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                                     bool enableDebugMode = false, Vector3 offsetPosition = new Vector3())
    {
        Transform boxTransform = boxCollider.transform;

        Vector3 center = boxTransform.TransformPoint(boxCollider.center) + offsetPosition;
        Vector3 halfSize = Vector3.Scale(boxCollider.size, boxTransform.lossyScale) / 2;

        if (enableDebugMode)
            Debug.DrawCube(center, boxTransform.rotation, halfSize, Color.yellow);

        return Physics.BoxCastAll(center, halfSize, direction, boxTransform.rotation, maxDistance, layerMask, queryTriggerInteraction)
            .Where(x => x.collider != boxCollider);
    }

    public static IEnumerable<Collider> Overlap(this SphereCollider sphereCollider,
                                     int layerMask = int.MaxValue,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                                     bool enableDebugMode = false, Vector3 offsetPosition = new Vector3())
    {
        Transform boxTransform = sphereCollider.transform;

        Vector3 center = boxTransform.TransformPoint(sphereCollider.center) + offsetPosition;
        float radius = sphereCollider.radius * MathExtentions.Max(boxTransform.lossyScale);

        if (enableDebugMode)
            Debug.DrawSphere(center, boxTransform.rotation, radius, Color.yellow);

        return Physics.OverlapSphere(center, radius, layerMask, queryTriggerInteraction).Where(x => x != sphereCollider);
    }

    public static IEnumerable<RaycastHit> CastAll(this SphereCollider sphereCollider,
                                     Vector3 direction,
                                     float maxDistance,
                                     int layerMask = int.MaxValue,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                                     bool enableDebugMode = false, Vector3 offsetPosition = new Vector3())
    {
        Transform boxTransform = sphereCollider.transform;

        Vector3 center = boxTransform.TransformPoint(sphereCollider.center) + offsetPosition;
        float radius = sphereCollider.radius * MathExtentions.Max(boxTransform.lossyScale);

        if (enableDebugMode)
            Debug.DrawSphere(center, boxTransform.rotation, radius, Color.yellow);

        return Physics.SphereCastAll(new Ray(center, direction), radius, maxDistance, layerMask, queryTriggerInteraction)
            .Where(x => x.collider != sphereCollider);
    }

    public static IEnumerable<Collider> Overlap(this CapsuleCollider capsuleCollider,
                                     int layerMask = int.MaxValue,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                                     bool enableDebugMode = false, Vector3 offsetPosition = new Vector3())
    {
        Transform capsuleTransform = capsuleCollider.transform;

        Vector3 localDirection = new Vector3 { [capsuleCollider.direction] = 1 };
        float offset = capsuleCollider.height / 2;

        Vector3 localPoint0 = capsuleCollider.center - localDirection * offset;
        Vector3 localPoint1 = capsuleCollider.center + localDirection * offset;

        Vector3 point0 = capsuleTransform.TransformPoint(localPoint0) + offsetPosition;
        Vector3 point1 = capsuleTransform.TransformPoint(localPoint1) + offsetPosition;


        Vector3 direction = capsuleTransform.rotation * localDirection;
        var r = Vector3.Scale(new Vector3(capsuleCollider.radius, capsuleCollider.radius, capsuleCollider.radius), capsuleTransform.lossyScale);

        float radius = Enumerable.Range(0, 3).Select(xyz => xyz == capsuleCollider.direction ? 0 : r[xyz])
                .Select(Mathf.Abs).Max();

        float factHeight = Vector3.Distance(point0, point1);
        float maxRadius = Mathf.Clamp(radius, 0, factHeight / 4);

        if (Vector3.Distance(point0 + direction, point1 - direction) < Vector3.Distance(point0 - direction, point1 + direction))
        {
            point0 += direction * maxRadius;
            point1 -= direction * maxRadius;
        }
        else
        {
            point0 -= direction * maxRadius;
            point1 += direction * maxRadius;
        }

        if (enableDebugMode)
        {
            Debug.DrawSphere(point0, capsuleTransform.rotation, radius, Color.blue);
            Debug.DrawSphere(point1, capsuleTransform.rotation, radius, Color.green);
            Debug.DrawLine(point0, point1, Color.red);
        }

        return Physics.OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction).Where(n => n != capsuleCollider);
    }

    public static IEnumerable<RaycastHit> CastAll(this CapsuleCollider capsuleCollider,
                                     Vector3 direction,
                                     float maxDistance,
                                     int layerMask = int.MaxValue,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                                     bool enableDebugMode = false, Vector3 offsetPosition = new Vector3())
    {
        Transform capsuleTransform = capsuleCollider.transform;

        Vector3 localDirection = new Vector3 { [capsuleCollider.direction] = 1 };
        float offset = capsuleCollider.height / 2;

        Vector3 localPoint0 = capsuleCollider.center - localDirection * offset;
        Vector3 localPoint1 = capsuleCollider.center + localDirection * offset;

        Vector3 point0 = capsuleTransform.TransformPoint(localPoint0) + offsetPosition;
        Vector3 point1 = capsuleTransform.TransformPoint(localPoint1) + offsetPosition;


        Vector3 capsuleDirection = capsuleTransform.rotation * localDirection;
        var r = Vector3.Scale(new Vector3(capsuleCollider.radius, capsuleCollider.radius, capsuleCollider.radius), capsuleTransform.lossyScale);

        float radius = Enumerable.Range(0, 3).Select(xyz => xyz == capsuleCollider.direction ? 0 : r[xyz])
                .Select(Mathf.Abs).Max();

        float factHeight = Vector3.Distance(point0, point1);
        float maxRadius = Mathf.Clamp(radius, 0, factHeight / 4);

        if (Vector3.Distance(point0 + capsuleDirection, point1 - capsuleDirection) < Vector3.Distance(point0 - capsuleDirection, point1 + capsuleDirection))
        {
            point0 += capsuleDirection * maxRadius;
            point1 -= capsuleDirection * maxRadius;
        }
        else
        {
            point0 -= capsuleDirection * maxRadius;
            point1 += capsuleDirection * maxRadius;
        }

        if (enableDebugMode)
        {
            Debug.DrawRay(capsuleTransform.position, direction, Color.yellow, 1f);
        }

        return Physics.CapsuleCastAll(point0, point1, radius, direction, maxDistance, layerMask, queryTriggerInteraction)
            .Where(n => n.collider != capsuleCollider);
    }
}
