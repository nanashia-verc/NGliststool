# NG管理ツール

Windows 向けの NG 管理ツールです。C# WinForms + SQLite を使い、案件登録・再検査・マスタ管理・CSV 出力・バックアップを行えます。

## 主な機能
- NG 案件の登録と一覧表示
- 検査履歴の追加と再検査フロー
- 型番・NG理由・処置内容のマスタ管理
- CSV 形式でのエクスポート
- SQLite データベースのバックアップ
- ログ出力とテストカバレッジ

## 前提条件
- .NET SDK 9.0 以降
- Windows 10/11

## 実行方法
```powershell
dotnet build NgProductManager.sln
dotnet test NgProductManager.sln
dotnet run --project src/NgProductManager/NgProductManager.csproj
```

## 配布版の利用方法
1. GitHub Releases から `NgProductManager-win-x64.zip` をダウンロードします。
2. 任意のフォルダへ展開します。
3. 展開先フォルダ内の `NgProductManager.exe` を起動します。
4. データベースはユーザーごとの AppData 配下に保存されるため、ZIP を更新しても通常は既存データを引き継ぎます。
5. コード署名なしのため、初回起動時に Windows Defender SmartScreen による警告が表示される場合があります。

## 配布用 publish
```powershell
dotnet publish src/NgProductManager/NgProductManager.csproj -c Release -r win-x64 -p:SelfContained=true -p:PublishSingleFile=true -p:PublishTrimmed=false -o artifacts/publish/win-x64
```

## プロジェクト構成
- src/NgProductManager: WinForms アプリ本体
- src/NgProductManager/Forms: 画面フォーム
- src/NgProductManager/Services: 業務ロジック
- src/NgProductManager/Data: SQLite リポジトリ・DB 管理
- src/NgProductManager/Models: ドメインモデル
- tests/NgProductManager.Tests: MSTest のテストコード

## 開発メモ
- UI は日本語表示を前提にしています。
- DB ファイルは実行環境の AppData 配下に保存されます。
- 追加変更時は `dotnet test NgProductManager.sln` で確認してください。
