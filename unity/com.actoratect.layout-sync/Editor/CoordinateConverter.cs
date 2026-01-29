using UnityEngine;

namespace Actoratect.LayoutSync
{
    public static class CoordinateConverter
    {
        /// <summary>
        /// Maya座標系からUnity座標系に変換
        /// Maya: Y-up, Right-handed, cm
        /// Unity: Y-up, Left-handed, m
        /// </summary>
        public static Vector3 MayaToUnityPosition(Vector3 mayaPos)
        {
            // cm → m に変換 & Z軸反転
            return new Vector3(
                mayaPos.x * 0.01f,
                mayaPos.y * 0.01f,
                -mayaPos.z * 0.01f
            );
        }

        /// <summary>
        /// Maya回転をUnity回転に変換
        /// </summary>
        public static Vector3 MayaToUnityRotation(Vector3 mayaRot)
        {
            return new Vector3(
                -mayaRot.x,
                -mayaRot.y,
                mayaRot.z
            );
        }

        /// <summary>
        /// Unity座標系からMaya座標系に変換
        /// </summary>
        public static Vector3 UnityToMayaPosition(Vector3 unityPos)
        {
            // m → cm に変換 & Z軸反転
            return new Vector3(
                unityPos.x * 100f,
                unityPos.y * 100f,
                -unityPos.z * 100f
            );
        }

        /// <summary>
        /// Unity回転をMaya回転に変換
        /// </summary>
        public static Vector3 UnityToMayaRotation(Vector3 unityRot)
        {
            return new Vector3(
                -unityRot.x,
                -unityRot.y,
                unityRot.z
            );
        }
    }
}