#!/bin/bash
# postCreateCommand で実行されるセットアップスクリプト

# エラーが発生したら即座に停止する
set -e

echo "Running post-create setup..."

# UnityやC#プロジェクトの初期化コマンドなどがあればここに追記します
# dotnet restore 等

echo "Setup completed successfully!"
