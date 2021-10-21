# CSharp-WPF-OpenTK
C# + WPF(.NetFramework) + OpenTK

# IDE
Visual Studio 2019

# 各プロジェクトの説明
1. キューブを表示する
2. キューブが回転するアニメーション
3. キーボードからの入力でキューブを動かす
4. マウスの操作でキューブを動かす

# ファイル
* MainWindow.xaml: 画面
* MainWindow.xaml.cs: 画面に対するソースコード

上記以外のファイルは設定ファイル。

# その他
## NuGetでOpenTKをインストールする
「ツール(T)」＞「NuGet パッケージマネージャー(N)」＞「ソリューションの NuGet パッケージの管理(N)...」
「参照」をクリックし検索ボックスに[OpenTK.GLControl]と入力する
![NuGetパッケージマネージャ](https://user-images.githubusercontent.com/77771651/111118029-9d4e7a80-85ab-11eb-8373-b5bbb33585b4.png)

## MainWindow.xamlにOpenGLの画面を挿入する
1. ツールボックス＞すべての WPF コントロール＞WindowsFormsHost をMainWindow上にドラッグする。
2. MainWindow.xaml.csを開き、XAMLコード内のWindowsFormsHostタグを下記のように編集する。    
x:Nameに名前を指定することで、C#のコードMainWindow.xaml.csから参照できるようになる。

```C#
<WindowsFormsHost x:Name="openGLHost" Margin="0,0,240,0"/>
```

![Add-WindowsFormsHost](https://user-images.githubusercontent.com/77771651/111118080-af301d80-85ab-11eb-91b2-bea9ae996e9f.png)
