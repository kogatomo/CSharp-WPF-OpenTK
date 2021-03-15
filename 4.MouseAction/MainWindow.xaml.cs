using System;
using System.Windows;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MouseAction
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //視点（カメラ）に関する設定----------------------------------------
        float cameraPositionX = 4.0f;
        float cameraPositionY = 4.0f;
        float cameraPositionZ = 4.0f;
        float cameraPositionZMin = 1.0f;
        float cameraPositionZMax = 30.0f;
        //------------------------------------------------------------------

        //マウスに関する設定------------------------------------------------
        //クリックした場所の座標
        int clickX, clickY;
        //回転行列
        Matrix4 rotate = Matrix4.Identity;
        //回転行列の初期値
        Matrix4 rotateInitial = Matrix4.Identity;
        //------------------------------------------------------------------

        GLControl glControl = new GLControl(GraphicsMode.Default);

        public MainWindow()
        {
            InitializeComponent();

            //イベントの追加 
            glControl.Load += glControl_Load;
            glControl.Resize += glControl_Resize;
            glControl.Paint += glControl_Paint;
            glControl.MouseDown += glControl_MouseDown;
            glControl.MouseUp += glControl_MouseUp;
            //glControl.MouseMove += glControl_MouseMove;//Ctrlキー＋マウスクリック時のみ有効になるようにした
            glControl.MouseWheel += glControl_MouseWheel;
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

            // マウス操作を受け付け視点を変更する。
            GL.MultMatrix(ref rotate);

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
        /// glControlがクリックされた時に実行される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Ctrlキーが押されている場合
                if (Control.ModifierKeys == Keys.Control)
                {
                    glControl.MouseMove += glControl_MouseMove;
                    //ドラッグ開始点を保存
                    clickX = e.X;
                    clickY = e.Y;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                //Ctrlキーが押されている場合
                if (Control.ModifierKeys == Keys.Control)
                {
                    glControl.MouseMove += glControl_MouseMove;
                    //ドラッグ開始点を保存
                    clickX = e.X;
                    clickY = e.Y;
                }
            }
        }

        /// <summary>
        /// マウスクリックが離れた時に実行される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            glControl.MouseMove -= glControl_MouseMove;
            //回転行列の初期値を更新
            rotateInitial = rotate;
        }

        /// <summary>
        /// マウスがglControl上を移動する時に実行される。
        /// 本関数はglControl上でCtrlキー＋マウスクリック時のみ処理される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            //マウスのボタンを長押ししているときの処理
            if (Control.ModifierKeys == Keys.Control)
            {
                MouseMotion(e.X, e.Y); //回転
                glControl.Invalidate();//再描画
            }
        }

        /// <summary>
        /// マウスドラッグ時の処理。
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public void MouseMotion(int moveX, int moveY)
        {
            float dx, dy, dnorm;

            //ドラッグ開始位置からの変位
            //変位はwidth,heightで割り正規化している
            dx = (moveX - clickX) / (float)glControl.Width;
            dy = (moveY - clickY) / (float)glControl.Height;

            //ドラッグ開始位置からの距離
            dnorm = (float)Math.Sqrt(dx * dx + dy * dy);

            if (dnorm != 0.0)
            {
                //マウスの移動距離を回転角に換算する
                float radian = dnorm * MathHelper.Pi * 0.5f;//d*π/2
                float spin = (float)Math.Sin(radian);       //sin(d*π/2)
                //マウスドラッグ時の回転のクォータニオンを求める
                //マウスをx方向に移動したときにy軸回りの回転、y方向に移動したときにx軸回りの回転にする
                Quaternion quaternion = new Quaternion(
                                                dy * spin,
                                                dx * spin,
                                                0.0f,
                                                (float)Math.Cos(radian));
                //回転させる
                rotate = rotateInitial * Matrix4.CreateFromQuaternion(quaternion);
            }
        }

        /// <summary>
        /// マウスのホイールを移動した時に実行される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            float wheel = e.Delta * 0.01f;

            cameraPositionZ += wheel;

            //これ以上近付かないようにする処理
            if (cameraPositionZ <= cameraPositionZMin) cameraPositionZ = cameraPositionZMin;
            //これ以上遠ざからないようにする処理
            if (cameraPositionZ >= cameraPositionZMax) cameraPositionZ = cameraPositionZMax;

            glControl.Invalidate();//再描画
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
    }
}
