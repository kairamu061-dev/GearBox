# ベース: Node.js 22 LTS (Claude Code に必要)
FROM node:22-bookworm-slim

# ========== システム依存パッケージ ==========
RUN apt-get update && apt-get install -y --no-install-recommends \
    # .NET Microsoft リポジトリの GPG 署名検証に必要
    gnupg \
    ca-certificates \
    apt-transport-https \
    software-properties-common \
    # 開発ツール
    git \
    curl \
    wget \
    bash \
    jq \
    unzip \
    procps \
    && rm -rf /var/lib/apt/lists/*

# ========== .NET 8 SDK ==========
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb \
    -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

# ========== Claude Code ==========
RUN npm install -g @anthropic-ai/claude-code

# ========== ワークスペース ==========
WORKDIR /workspace

# node ユーザーで実行（root を避けセキュリティ向上）
USER node

# Claude Code と Antigravity の設定・記憶ディレクトリを作成
RUN mkdir -p /home/node/.claude /home/node/.gemini
