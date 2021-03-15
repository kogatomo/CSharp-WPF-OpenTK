using System;
using System.Windows;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace KeyboardAction
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //チェックボックス--------------------------------------------------
        bool isAbsoluteCoordinateSystemOn = false;
        //------------------------------------------------------------------

        //視点（カメラ）に関する設定----------------------------------------
        float cameraPositionX = 4.0f;
        float cameraPositionY = 4.0f;
        float cameraPositionZ = 4.0f;
        float rotX = 0.0f, rotY = 0.0f, rotZ = 0.0f;       //回転角初期値
        float transX = 0.0f, transY = 0.0f, transZ = 0.0f; //視点の位置
        float displayScale = 1.0f;                         //表示スケール
        float rotX_dev = 5.0f, rotY_dev = 5.0f, rotZ_dev = 5.0f; //回転角増分
        float trans_dev = 0.2f;                                  //視点増分
        float displayScale_dev = 1.2f;                           //表示スケール増分
        //------------------------------------------------------------------

        GLControl glControl = new GLControl(GraphicsMode.Default);

        public MainWindow()
        {
            InitializeComponent();

            //イベントの追加 
            glControl.Load += glControl_Load;
            glControl.Resize += glControl_Resize;
            glControl.Paint += glControl_Paint;
            glControl.KeyPress += glControl_KeyPress;
            //ホストの子に設定 
            openGLHost.Child = glControl;
        }

        /// <summary>
        /// glControlが初めて表示される前に実行される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_Load(object sender, EventArgs e)
        {
            //背景色の設定
            GL.ClearColor(Color4.Gray);

            //デプスバッファの使用
            GL.Enable(EnableCap.DepthTest);

            //法線の正規化
            GL.Enable(EnableCap.Normalize);

            //カリング（裏面削除、反時計回りを表に設定）
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
        }

        /// <summary>
        /// glControlのサイズ変更時に実行される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_Resize(object sender, EventArgs e)
        {
            // ビューポート（画像が実際に描画される領域）の設定-------------
            //  (x, y)は左下隅の座標，(width, height)はサイズ。
            //  OpenGLでは左下隅が(0, 0)で上向きが＋。Formでは左上隅が(0, 0)。
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            //--------------------------------------------------------------

            //視体積（視野）の設定
            SetProjection();
        }

        /// <summary>
        /// glControlの描画・再描画時に実行される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            //バッファの初期化（描画の最初に必ず実行する）
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //ビューポート（画像が実際に描画される領域）の設定--------------
            // (x, y)は左下隅の座標，(width, height)はサイズ。
            // OpenGLでは左下隅が(0, 0)で上向きが＋。Formでは左上隅が(0, 0)。
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            //--------------------------------------------------------------

            //視体積（視野）の設定
            SetProjection();

            //視界（カメラの位置、姿勢）の設定
            SetModelView();

            //キー操作を受け付け視点を変更する。
            ChangeVeiwPoint();

            //絶対座標系の描画
            if (isAbsoluteCoordinateSystemOn) AbsoluteCoordinateSystem.Draw();

            //モデルの描画--------------------------------------------------
            DrawCuboid(1.0f, 1.0f, 1.0f);
            //--------------------------------------------------------------

            glControl.SwapBuffers();
        }

        /// <summary>
        /// 視体積（視野）を設定する。
        /// </summary>
        private void SetProjection()
        {
            GL.MatrixMode(MatrixMode.Projection);

            Matrix4 cpfov = Matrix4.CreatePerspectiveFieldOfView(
                //y方向の視野の角度[rad]（縦方向の視野角）
                MathHelper.DegreesToRadians(45),
                //(ビュー領域の幅÷高さ)として定義される縦横比（水平方向の視野角）
                glControl.AspectRatio,
                //ニアビュー平面までの距離（手前）
                1.0f,
                //ファービュー平面までの距離（奥行き）
                100.0f);
            GL.LoadMatrix(ref cpfov);
        }

        /// <summary>
        /// 視界（カメラの位置、視界の中心位置、視界の上方向）を設定する。
        /// </summary>
        private void SetModelView()
        {
            GL.MatrixMode(MatrixMode.Modelview);

            Matrix4 lookAt = Matrix4.LookAt(
                //カメラの位置
                new Vector3(cameraPositionX, cameraPositionY, cameraPositionZ),
                //視界の中心位置（x,y,z）
                new Vector3(0.0f, 0.0f, 0.0f),
                //視界の上方向をy軸方向に指定
                new Vector3(0.0f, 1.0f, 0.0f));
            GL.LoadMatrix(ref lookAt);
        }

        /// <summary>
        /// キー入力時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'x': rotX = (rotX + rotX_dev) % 360.0f; break;
                case 'X': rotX = (rotX - rotX_dev) % 360.0f; break;
                case 'y': rotY = (rotY + rotY_dev) % 360.0f; break;
                case 'Y': rotY = (rotY - rotY_dev) % 360.0f; break;
                case 'z': rotZ = (rotZ + rotZ_dev) % 360.0f; break;
                case 'Z': rotZ = (rotZ - rotZ_dev) % 360.0f; break;
                case 'a': transX += trans_dev; break;
                case 'A': transX -= trans_dev; break;
                case 'b': transZ += trans_dev; break;
                case 'B': transZ -= trans_dev; break;
                case 'c': transY += trans_dev; break;
                case 'C': transY -= trans_dev; break;
                case 'i': cameraPositionZ -= 10.0f; break;
                case 'o': cameraPositionZ += 10.0f; break;
                case 'S': displayScale *= displayScale_dev; break;
                case 's': displayScale /= displayScale_dev; break;
                case 'q': Environment.Exit(0); break;
                default:
                    break;
            }
            glControl.Invalidate();//再描画
        }
        /// <summary>
        /// 視点を変更する。
        /// </summary>
        void ChangeVeiwPoint()
        {
            //回転
            GL.Rotate(rotX, 1.0, 0.0, 0.0);
            GL.Rotate(rotY, 0.0, 1.0, 0.0);
            GL.Rotate(rotZ, 0.0, 0.0, 1.0);
            //スケール変更
            GL.Scale(displayScale, displayScale, displayScale);
            //視点変更
            GL.Translate(transX, transY, transZ);
        }

        /// <summary>
        /// 直方体を描画する。
        /// </summary>
        /// <param name="x">直方体中心点からの距離x</param>
        /// <param name="y">直方体中心点からの距離y</param>
        /// <param name="z">直方体中心点からの距離z</param>
        private void DrawCuboid(float x, float y, float z)
        {
            // 直方体の定義
            //      _________
            //     /   上   /|
            //    /_______ / |
            // 左 |        | | 右
            //    |   前   | |
            //    |________|/
            //        下
            GL.Begin(PrimitiveType.Quads);
            //直方体の右面 X
            GL.Color4(Color4.Red);
            GL.Vertex3( x,-y, -z);
            GL.Vertex3( x, y, -z);
            GL.Vertex3( x, y,  z);
            GL.Vertex3( x,-y,  z);
            //直方体の上面 Y
            GL.Color4(Color4.Green);
            GL.Vertex3(-x, y, -z);
            GL.Vertex3(-x, y,  z);
            GL.Vertex3( x, y,  z);
            GL.Vertex3( x, y, -z);
            //直方体の前面 Z
            GL.Color4(Color4.Blue);
            GL.Vertex3(-x, -y, z);
            GL.Vertex3( x, -y, z);
            GL.Vertex3( x,  y, z);
            GL.Vertex3(-x,  y, z);
            //直方体の左面 -X
            GL.Color4(Color4.DarkRed);
            GL.Vertex3(-x, -y, -z);
            GL.Vertex3(-x, -y,  z);
            GL.Vertex3(-x,  y,  z);
            GL.Vertex3(-x,  y, -z);
            //直方体の下面 -Y
            GL.Color4(Color4.DarkGreen);
            GL.Vertex3(-x, -y, -z);
            GL.Vertex3( x, -y, -z);
            GL.Vertex3( x, -y,  z);
            GL.Vertex3(-x, -y,  z);
            //直方体の裏面 -Z
            GL.Color4(Color4.DarkBlue);
            GL.Vertex3(-x, -y, -z);
            GL.Vertex3(-x,  y, -z);
            GL.Vertex3( x,  y, -z);
            GL.Vertex3( x, -y, -z);
            GL.End();
        }

        //チェックボックス--------------------------------------------------------------
        private void AbsoluteCoordinateSystem_Checked(object sender, RoutedEventArgs e)
        {
            isAbsoluteCoordinateSystemOn = true;
            glControl.Invalidate();//再描画
        }
        private void AbsoluteCoordinateSystem_UnChecked(object sender, RoutedEventArgs e)
        {
            isAbsoluteCoordinateSystemOn = false;
            glControl.Invalidate();//再描画
        }
        //------------------------------------------------------------------------------

     }
}
