using SharpDX;

namespace DSharpDXRastertek.Series2.TutTerr13.Graphics.Data
{
    public class DFrustum                   // 77 lines
    {
        // Variables
        private float m_ScreenDepth;
        public Plane[] _Planes = new Plane[6];

        // Methods
        public void Initialize(float screenDepth)
        {
            m_ScreenDepth = screenDepth;
        }
        public void ConstructFrustum(Matrix projection, Matrix view)
        {
            // Calculate the minimum Z distance in the frustum.
            float zMinimum = -projection.M43 / projection.M33;
            float r = m_ScreenDepth / (m_ScreenDepth - zMinimum);

            // Load the updated values back into the projection matrix.
            projection.M33 = r;
            projection.M43 = -r * zMinimum;

            // Create the frustum matrix from the view matrix and updated projection matrix.
            Matrix matrix = view * projection;

            // Calculate near plane of frustum.
            _Planes[0] = new Plane(matrix.M14 + matrix.M13, matrix.M24 + matrix.M23, matrix.M34 + matrix.M33, matrix.M44 + matrix.M43);
            _Planes[0].Normalize();

            // Calculate far plane of frustum.
            _Planes[1] = new Plane(matrix.M14 - matrix.M13, matrix.M24 - matrix.M23, matrix.M34 - matrix.M33, matrix.M44 - matrix.M43);
            _Planes[1].Normalize();

            // Calculate left plane of frustum.
            _Planes[2] = new Plane(matrix.M14 + matrix.M11, matrix.M24 + matrix.M21, matrix.M34 + matrix.M31, matrix.M44 + matrix.M41);
            _Planes[2].Normalize();

            // Calculate right plane of frustum.
            _Planes[3] = new Plane(matrix.M14 - matrix.M11, matrix.M24 - matrix.M21, matrix.M34 - matrix.M31, matrix.M44 - matrix.M41);
            _Planes[3].Normalize();

            // Calculate top plane of frustum.
            _Planes[4] = new Plane(matrix.M14 - matrix.M12, matrix.M24 - matrix.M22, matrix.M34 - matrix.M32, matrix.M44 - matrix.M42);
            _Planes[4].Normalize();

            // Calculate bottom plane of frustum.
            _Planes[5] = new Plane(matrix.M14 + matrix.M12, matrix.M24 + matrix.M22, matrix.M34 + matrix.M32, matrix.M44 + matrix.M42);
            _Planes[5].Normalize();
        }
        public bool CheckPoint(float x, float y, float z)
        {
            return CheckPoint(new Vector3(x, y, z));
        }
        private bool CheckPoint(Vector3 point)
        {
            // Check if the point is inside all six planes of the view frustum.
            for (var i = 0; i < 6; i++)
                if (Plane.DotCoordinate(_Planes[i], point) <= 0f)
                    return false;

            return true;
        }
        public bool CheckCube(Vector3 center, float radius)
        {
            return CheckCube(center.X, center.Y, center.Z, radius);
        }
        private bool CheckCube(float xCenter, float yCenter, float zCenter, float radius)
        {
            // Check if any one point of the cube is in the view frustum.
            for (var i = 0; i < 6; i++)
            {
                // Check all eight points of the cube to see if they all reside within the frustum.
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - radius, yCenter - radius, zCenter - radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + radius, yCenter - radius, zCenter - radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - radius, yCenter + radius, zCenter - radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + radius, yCenter + radius, zCenter - radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - radius, yCenter - radius, zCenter + radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + radius, yCenter - radius, zCenter + radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - radius, yCenter + radius, zCenter + radius)) > 0.0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + radius, yCenter + radius, zCenter + radius)) > 0.0f)
                    continue;

                return false;
            }
            return true;
        }
        public bool CheckSphere(Vector3 center, float radius)
        {
            // Check if the radius of the sphere is inside the view frustum.
            for (int i = 0; i < 6; i++)
            {
                if (Plane.DotCoordinate(_Planes[i], center) <= -radius)
                    return false;
            }
            return true;
        }
        private bool CheckSphere(float x, float y, float z, float radius)
        {
            return CheckSphere(new Vector3(x, y, z), radius);
        }
        public bool CheckRectangle(Vector3 center, Vector3 size)
        {
            return CheckRectangle(center.X, center.Y, center.Z, size.X, size.Y, size.Z);
        }
        private bool CheckRectangle(float xCenter, float yCenter, float zCenter, float xSize, float ySize, float zSize)
        {
            // Check if any of the 6 planes of the rectangle are inside the view frustum.
            for (var i = 0; i < 6; i++)
            {
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - xSize, yCenter - ySize, zCenter - zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + xSize, yCenter - ySize, zCenter - zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - xSize, yCenter + ySize, zCenter - zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + xSize, yCenter + ySize, zCenter - zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - xSize, yCenter - ySize, zCenter + zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + xSize, yCenter - ySize, zCenter + zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter - xSize, yCenter + ySize, zCenter + zSize)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(xCenter + xSize, yCenter + ySize, zCenter + zSize)) >= 0f)
                    continue;

                return false;
            }

            return true;
        }
        public bool CheckRectangle2(float maxWidth, float maxHeight, float maxDepth, float minWidth, float minHeight, float minDepth)
        {
            // Check if any of the 6 planes of the rectangle are inside the view frustum.
            for (var i = 0; i < 6; i++)
            {
                if (Plane.DotCoordinate(_Planes[i], new Vector3(minWidth, minHeight, minDepth)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(maxWidth, minHeight, minDepth)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(minWidth, maxHeight, minDepth)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(maxWidth, maxHeight, minDepth)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(minWidth, minHeight, maxDepth)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(maxWidth, minHeight, maxDepth)) >= 0f)
                    continue;
                 if (Plane.DotCoordinate(_Planes[i], new Vector3(minWidth, maxHeight, maxDepth)) >= 0f)
                    continue;
                if (Plane.DotCoordinate(_Planes[i], new Vector3(maxWidth, maxHeight, maxDepth)) >= 0f)
                    continue;

                return false;
            }

            return true;
        }
    }
}