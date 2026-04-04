using UnityEngine;

[System.Serializable]
public class SpringBone
{
    public Transform transform;

    [HideInInspector] public Vector3 currentDir;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 restDir;
    [HideInInspector] public Quaternion baseLocalRotation;
}

public class SpringBoneRotator : MonoBehaviour
{
    public SpringBone[] bones;

    [Header("Spring Settings")]
    public float stiffness = 200f;
    public float damping = 10f;
    public float inertia = 1f;

    [Header("External Forces")]
    public Vector3 gravity = new Vector3(0, -1, 0);
    public Rigidbody controllingBody;

    void Start()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            var bone = bones[i];

            bone.baseLocalRotation = bone.transform.localRotation;

            // Determine rest direction (towards child or forward)
            if (bone.transform.childCount > 0)
                bone.restDir = (bone.transform.GetChild(0).localPosition).normalized;
            else
                bone.restDir = Vector3.forward;

            bone.currentDir = bone.restDir;
            bone.velocity = Vector3.zero;
        }
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < bones.Length; i++)
        {
            var bone = bones[i];

            // Convert rest direction to world
            Vector3 worldRestDir = bone.transform.parent.TransformDirection(bone.restDir);

            // Add forces
            Vector3 force = (worldRestDir - bone.currentDir) * stiffness;
            force += gravity;
            force -= bone.velocity * damping;
            force += transform.InverseTransformDirection(controllingBody.velocity) * inertia;

            // Integrate
            bone.velocity += force * dt;
            bone.currentDir += bone.velocity * dt;
            bone.currentDir.Normalize();

            // Convert back to local rotation
            Vector3 localDir = bone.transform.parent.InverseTransformDirection(bone.currentDir);

            Quaternion rot = Quaternion.FromToRotation(bone.restDir, localDir);
            bone.transform.localRotation = rot * bone.baseLocalRotation;
        }
    }
}