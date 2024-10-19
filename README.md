# KScriptWin
##  C言語風のスクリプト言語とその実行環境

KScript はC言語に似せたスクリプト言語でインタプリタとして動作する。
自前のCADソフト(CadAppやMin3DCad)に組み込んで動作させることを目的に作成した。
基本的な制御構造と型指定のない変数で作られている。
KScriptWin はWindows上でKScriptのコード作成とそれを実行する環境である。
  
使い方などは[説明書](Document/KScriptWinManual.pdf)を参照。  
実行方法は[KScriptWin.zip](KScriptWin.zip)をダウンロードし適当なフォルダーに展開して KScriptWin.exe を実行する。  
<img src="Image/zip_DownLoad_commnet.png" width="80%">

### 画面
メインウィンドウ    
<img src="Image/MainWindow.png" width="80%">  

グラフィック表示  
<img src="Image/MainWindow+Graph.png" width="80%">  


### 履歴
2024/10/06 グラフィック関数  
2024/10/13 ファイル参照機能  
2024/10/06 ダイヤログ関数 (input,message)  
2024/10/04 中断、一時停止機能  
2024/09/30 エディタのフォント選択、検索、  
2024/09/29 スクリプトの作成にAvalonEditorを使用,print文をコールバック関数にして実行側で出力先を選択  
2024/09/27 Console版から移植で作成開始  

### ■実行環境
[KScriptWin.zip](KScriptWin.zip)をダウンロードして適当なフォルダに展開し、フォルダ内の KScriptWin.exe をダブルクリックして実行します。  
動作環境によって「.NET 8.0 Runtime」が必要になる場合もあります。  
https://dotnet.microsoft.com/ja-jp/download


### ■開発環境  
開発ソフト : Microsoft Visual Studio 2022  
開発言語　 : C# 10.0 Windows アプリケーション  
フレームワーク　 :  .NET 8.0  
NuGetライブラリ : AvalonEditor  
自作ライブラリ  : CoreLib (express,YWordDraw,YDraw,InputBox,MenuDialog)  
