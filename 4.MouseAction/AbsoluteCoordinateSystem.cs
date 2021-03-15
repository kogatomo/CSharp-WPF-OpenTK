using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MouseAction
{
    class AbsoluteCoordinateSystem
    {
        static void Tube(float length, float radius1, float radius2)
        {
            float rx, ry;
            int degree;

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Normal3(Vector3.One);
            for (degree = 0; degree <= 360; degree += 10)
            {
                rx = (float)Math.Cos(MathHelper.DegreesToRadians(degree));
                ry = (float)Math.Sin(MathHelper.DegreesToRadians(degree));
                GL.Vertex3(rx * radius2, ry * radius2, length);
                GL.Vertex3(rx * radius1, ry * radius1, 0.0f);

            }
            GL.End();
        }

        /// <summary>
        /// 絶対座標系
        /// </summary>
        public static void Draw()
        {
            float height = 2.0f;
            float height_cone = 0.2f;
            float radius = 0.02f;
            float radius_cone = 0.06f;

            // X
            GL.PushMatrix();
            GL.Color4(Color4.DarkRed);
            GL.Rotate(90.0, 0.0, 1.0, 0.0);
            Tube(height, radius, radius);
            GL.Translate(0.0f, 0.0f, height);
            Tube(height_cone, radius_cone, 0.0f);
            GL.PopMatrix();

            // Y
            GL.PushMatrix();
            GL.Color4(Color4.DarkGreen);
            GL.Rotate(-90.0, 1.0, 0.0, 0.0);
            Tube(height, radius, radius);
            GL.Translate(0.0f, 0.0f, height);
            Tube(height_cone, radius_cone, 0.0f);
            GL.PopMatrix();

            // Z
            GL.PushMatrix();
            GL.Color4(Color4.DarkBlue);
            Tube(height, radius, radius);
            GL.Translate(0.0f, 0.0f, height);
            Tube(height_cone, radius_cone, 0.0f);
            GL.PopMatrix();
        }
    }
}
