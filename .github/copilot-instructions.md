# Copilot instructions for NG management tool

- このリポジトリは C# WinForms + SQLite の Windows デスクトップアプリです。
- 変更時は日本語 UI と既存のレイヤード構造を維持してください。
- 主要な責務は以下の 4 層で分ける方針です: Forms / Services / Data / Models。
- SQLite の接続は毎回確実に閉じ、テストでは一時 DB を使うようにしてください。
- 変更後は `dotnet test NgProductManager.sln` を実行して確認してください。
- 既存のテストケース名や振る舞いを壊さないように注意してください。
